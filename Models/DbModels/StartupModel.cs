﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StartupsBack.Models.DbModels
{
    public class StartupModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime StartupPublished { get; set; }

        public int AuthorForeignKey { get; set; }

        public UserModel? Author { get; set; }

        [JsonIgnore]
        public byte[] Picture { get; set; } = Array.Empty<byte>();
        public string StartupPicFileName { get; set; } = string.Empty;
        public string Viewers { get; set; } = string.Empty;
        public DateTime LastModify { get; set; }

        public List<UserModel> Contributors { get; set; } = new List<UserModel>();

        public List<UserModel> WantToJoin { get; set; } = new List<UserModel>();
        public List<UserModel> AccsessDenied { get; set; } = new List<UserModel>();

    }
}
