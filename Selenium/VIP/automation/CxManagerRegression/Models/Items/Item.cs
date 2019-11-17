using System.Collections.Generic;
using Models.Interfaces;
using Models.Items.PushableApps;
using Models.Items.SchemaModels;
using Models.Items.TypeModels;
using Models.Users;
using Newtonsoft.Json;

namespace Models.Items
{
    public class Item : IEntity, IApprovalEntity
    {
        public long? Id { get; set; }
        public string SerialNumber { get; set; }
        public string CreateDate { get; set; }
        public UserIdName CreatedBy { get; set; }
        public string UpdateDate { get; set; }
        public UserIdName UpdatedBy { get; set; }
        public int Status { get; set; }
        public string Search { get; set; }
        public string Picture { get; set; }
        public string JsonData { get; set; }
        [JsonIgnore]
        public string JsonDataTitle { get; set; }
        public List<string> ModelReordering { get; set; }
        public string CustomData { get; set; }
        public long? ItemStoreId { get; set; }
        public SchemaModel SchemaModel { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TypeModel TypeModel {get; set;}
        public LangJsonData.LangJsonData LangJsonData { get; set; }
        public List<PushableApp> PushableApps { get; set; }
        public References.References References { get; set; }
    }
}
