using System;
using System.Linq;
using ApiModel = ListApp.Api.Models;

namespace ListApp.Models.Extensions
{
    public static class ModelExtensions
    {
        public static List ToLocalModel(this ApiModel.List cloudList)
        {
            var list = new List
            {
                ListId = cloudList.Guid,
                Name = cloudList.Name,
                IsDeleted = cloudList.IsDeleted,
                Index = cloudList.Index,
                CreationDate = cloudList.CreationDate,
                LastChangedDate = DateTime.UtcNow
            };

            foreach (var cloudListItem in cloudList.ListItems)
            {
                list.ListItems.Add(cloudListItem.ToLocalModel());
            }

            return list;
        }
        public static ListItem ToLocalModel(this ApiModel.ListItem cloudListItem)
        {
            var localListItem = new ListItem
            {
                Id = cloudListItem.Guid,
                Text = cloudListItem.Text,
                Description = cloudListItem.Description,
                Index = cloudListItem.Index,
                IsDeleted = cloudListItem.IsDeleted,
                Checked = cloudListItem.Checked,
                CreationDate = cloudListItem.CreationDate,
                LastChangedDate = DateTime.UtcNow
            };

            return localListItem;
        }
        public static ApiModel.ListItem ToApiModel(this ListItem localListItem, long apiListId = 0L)
        {
            var cloudListItem = new ApiModel.ListItem
            {
                ListId = apiListId,
                Guid = localListItem.Id,
                Text = localListItem.Text,
                Description = localListItem.Description,
                Index = localListItem.Index,
                IsDeleted = localListItem.IsDeleted,
                Checked = localListItem.Checked,
                CreationDate = localListItem.CreationDate,
                LastChangedDate = DateTime.UtcNow
            };

            return cloudListItem;
        }

        public static ApiModel.List ToApiModel(this List localList)
        {
            var cloudList = new ApiModel.List();
            cloudList.Guid = localList.ListId;
            cloudList.Name = localList.Name;
            cloudList.IsDeleted = localList.IsDeleted;
            cloudList.Index = localList.Index;
            cloudList.OwnerEmail = ApplicationUser.Current.Email;
            cloudList.CreationDate = localList.CreationDate;
            cloudList.LastChangedDate = DateTime.UtcNow;

            foreach (var localListItem in localList.ListItems)
            {
                cloudList.ListItems.Add(localListItem.ToApiModel());
            }

            return cloudList;
        }
        public static void CopyFrom(this ApiModel.List cloudList, List localList)
        {
            cloudList.Name = localList.Name;
            cloudList.IsDeleted = localList.IsDeleted;
            cloudList.Index = localList.Index;

            foreach (var localListItem in localList.ListItems)
            {
                if (cloudList.ListItems.FirstOrDefault(x => x.Guid == localListItem.Id) is ApiModel.ListItem cloudListItem)
                {
                    cloudListItem.IsDeleted = localListItem.IsDeleted;
                    cloudListItem.Text = localListItem.Text;
                    cloudListItem.Checked = localListItem.Checked;
                    cloudListItem.Description = localListItem.Description;
                    cloudListItem.Index = localListItem.Index;
                    cloudListItem.LastChangedDate = DateTime.UtcNow;
                }
                else
                {
                    cloudList.ListItems.Add(localListItem.ToApiModel(cloudList.Id));
                }
            }
        }
    }
}
