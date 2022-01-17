using AutoMapper;
using Microsoft.Azure.Documents;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Extensions;
using RyderGyde.ShopNotes.RedisUpdateTrigger.MappingProfile;
using System;
using System.Collections.Generic;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.Dtos
{
    public class VehicleShopNoteDto : IMapFrom<Document>
    {
        public string VehicleNumber { get; set; }
        public long CaseId { get; set; }
        public long AppointmentId { get; set; }
        public long EstimateId { get; set; }
        public IList<long> TaskIds { get; set; }
        public string Note { get; set; }
        public string NoteDate { get; set; }
        public string NoteType { get; set; }
        public string NoteAudience { get; set; }
        public string NoteUser { get; set; }
        public string ActionRequired { get; set; } = "N";
        public int Version { get; set; }

        public static void Mapping(Profile profile)
        {
            profile.CreateMap<Document, VehicleShopNoteDto>()
                .AfterMap((src, dest) =>
                {
                    foreach (var property in dest.GetType().GetProperties())
                    {
                        var value = src.GetValueFromMethod("GetPropertyValue", new Type[] { property.PropertyType }, property.Name) ?? dest.GetValue(property.Name);
                        dest.SetValue(property.Name, value);
                    }
                })
                ;
        }
    }
}
