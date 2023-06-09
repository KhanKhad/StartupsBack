﻿using Microsoft.EntityFrameworkCore;
using StartupsBack.Database;
using Microsoft.Extensions.Logging;
using StartupsBack.Models.DbModels;
using StartupsBack.ViewModels.ActionsResults;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace StartupsBack.ViewModels
{
    public class MessagesManagmentViewModel
    {
        private readonly ILogger _logger;
        private readonly MainDb _dbContext;

        public MessagesManagmentViewModel(ILogger logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<SendMessageResult> SendMessageAsync(string message, string senderName, string recipientName, string hash)
        {
            try
            {
                var sender = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Name == senderName);

                if (sender == null)
                    return SendMessageResult.AuthorNotFound();

                var myHash = await CalculateHash(senderName, sender.Token);

                if (myHash != hash)
                    return SendMessageResult.AuthenticationFailed();

                var recipient = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Name == recipientName);

                if (recipient == null)
                    return SendMessageResult.RecipientNotFound();

                var messageModel = new MessageModel()
                {
                    Message = message,
                    Recipient = recipient,
                    Sender = sender,
                    MessageSended = DateTime.UtcNow,
                    RecipientDelta = ++recipient.Delta,
                    SenderDelta = ++sender.Delta,
                };

                var res = await _dbContext.MessagesDB.AddAsync(messageModel);
                await _dbContext.SaveChangesAsync();

                return SendMessageResult.Success(res.Entity);
            }
            catch (Exception ex)
            {
                return SendMessageResult.UnknownError(ex);
            }
        }

        public async Task<GetMessagesResult> GetMessagesAsync(int id, string hash, int delta)
        {
            try
            {
                var author = await _dbContext.UsersDB.Include(i=>i.GettedMessages).Include(i => i.SendedMessages)
                    .FirstOrDefaultAsync(user => user.Id == id);

                if (author == null)
                    return GetMessagesResult.UserNotFound();

                var myHash = await CalculateHash(author.Name, author.Token);

                if (myHash != hash)
                    return GetMessagesResult.AuthenticationFailed();

                var result = author.GettedMessages.Where(i=>i.RecipientDelta > delta).ToList();
                result.AddRange(author.SendedMessages.Where(i=>i.SenderDelta > delta));

                return GetMessagesResult.Success(result.ToArray());
            }
            catch (Exception ex)
            {
                return GetMessagesResult.UnknownError(ex);
            }
        }

        public async Task<GetDeltaResult> GetDelta(int id)
        {
            try
            {
                var author = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Id == id);
                
                if (author == null) return GetDeltaResult.UserNotFound();

                return GetDeltaResult.Success(author.Delta);
            }
            catch (Exception ex)
            {
                return GetDeltaResult.UnknownError(ex);
            }
        }


        private const string _hashKey = "It's my message!";
        private static async Task<string> CalculateHash(string authorName, string authorToken)
        {
            using SHA256 mySHA256 = SHA256.Create();
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(authorName + authorToken + _hashKey));
            var byteResult = await mySHA256.ComputeHashAsync(stream);
            return Convert.ToBase64String(byteResult).Replace("+", "").Replace("/", "");
        }
    }
}
