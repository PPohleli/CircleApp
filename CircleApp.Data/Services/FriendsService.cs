﻿using Azure.Core;
using CircleApp.Data.Dtos;
using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly AppDbContext _context;

        public FriendsService(AppDbContext context)
        {
            _context = context;
        }


        public async Task SendRequestAsync(int senderId, int receiverId)
        {
            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = FriendshipStatus.Pending,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };
            _context.FriendRequests.Add(request);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateRequestAsync(int requestId, string newStatus)
        {
            var requestDb = await _context.FriendRequests.FirstOrDefaultAsync(n => n.Id == requestId);
            if (requestDb != null)
            {
                requestDb.Status = newStatus;
                requestDb.DateUpdated = DateTime.UtcNow;
                _context.Update(requestDb);
                await _context.SaveChangesAsync();
            }

            if (newStatus == FriendshipStatus.Accepted) 
            {
                var friendship = new Friendship
                {
                    SenderId = requestDb.SenderId,
                    ReceiverId = requestDb.ReceiverId,
                    DateCreated = DateTime.UtcNow,
                };
                await _context.Friendships.AddAsync(friendship);

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFriendAsync(int friendshipId)
        {
            var friendshipDb = await _context.Friendships.FirstOrDefaultAsync(n => n.Id == friendshipId);
            if (friendshipDb != null) 
            {
                _context.Friendships.Remove(friendshipDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId)
        {
            //get existing friends
            var existingFriendIds = await _context.Friendships.Where(n => n.SenderId == userId || n.ReceiverId == userId)
                .Select(n => n.SenderId == userId ? n.ReceiverId : n.SenderId).ToListAsync();

            //get pending request
            var pendingRequestIds = await _context.FriendRequests
                .Where(n => (n.SenderId == userId || n.ReceiverId == userId) && n.Status == FriendshipStatus.Pending)
                .Select(n => n.SenderId == userId ? n.ReceiverId : n.SenderId)
                .ToListAsync();

            //get suggested friends
            var suggestedFriends = await _context.Users.Where(n => n.Id != userId && !existingFriendIds.Contains(n.Id) && !pendingRequestIds.Contains(n.Id))
                .Select(u => new UserWithFriendsCountDto()
                {
                    User = u,
                    FriendsCount = _context.Friendships.Count(f => f.SenderId == u.Id || f.ReceiverId == u.Id),
                })
                .Take(5)
                .ToListAsync();

            return suggestedFriends;
        }
    }
}
