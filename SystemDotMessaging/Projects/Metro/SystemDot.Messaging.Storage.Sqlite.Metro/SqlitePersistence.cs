﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemDot.Logging;
using SystemDot.Messaging.Channels;
using SystemDot.Messaging.Channels.Packaging;
using SystemDot.Messaging.Channels.Packaging.Headers;
using SystemDot.Serialisation;
using SQLite;

namespace SystemDot.Messaging.Storage.Sqlite.Metro
{
    public class SqlitePersistence : IPersistence
    {
        readonly ISerialiser serialiser;

        public SqlitePersistence(ISerialiser serialiser)
        {
            this.serialiser = serialiser;
        }

        public IEnumerable<MessagePayload> GetMessages(EndpointAddress address)
        {
            return GetForChannelAsync(address).Result.Select(m => 
                new MessagePayload
                {
                    Id = new Guid(m.Id),
                    Headers = this.serialiser.Deserialise(m.Headers).As<List<IMessageHeader>>()
                });
        }

        async Task<List<MessagePayloadStorageItem>> GetForChannelAsync(EndpointAddress address)
        {
            var addressString = address.ToString();

            return await GetConnection()
                .Table<MessagePayloadStorageItem>()
                .Where(m => m.Address == addressString)
                .ToListAsync();
        }

        public async void StoreMessage(MessagePayload message, EndpointAddress address)
        {
            Logger.Info("Storing message in sqlite storage");

            const string statement = "insert or replace into MessagePayloadStorageItem" 
                + "(id, headers, address)" 
                + "values(?, ?, ?)";

            await GetConnection().ExecuteAsync(
                statement,
                message.Id.ToString(), 
                message.GetFromAddress().ToString(), 
                this.serialiser.Serialise(message.Headers));
        }

        public async void RemoveMessage(Guid id)
        {
            await GetConnection().ExecuteAsync("delete from MessagePayloadStorageItem where id = ?", id.ToString());
        }

        private static SQLiteAsyncConnection GetConnection()
        {
            return new SQLiteAsyncConnection("Messaging");
        }

        public async void Initialise()
        {
            await GetConnection().ExecuteAsync(
                "create table if not exists MessagePayloadStorageItem(\n"
                + "Id varchar(140) primary key not null ,\n"
                + "Headers blob ,\n"
                + "Address varchar(1000))");
        }
    }
}
