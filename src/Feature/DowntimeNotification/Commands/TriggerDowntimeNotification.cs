﻿using DowntimeNotification.Helpers;
using DowntimeNotification.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System.Linq;

namespace DowntimeNotification.Commands
{
    public class TriggerDowntimeNotification : Command
    {
        public override void Execute(CommandContext context)
        {
            Context.ClientPage.Start((object)this, "Run");
        }
        protected virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (!args.HasResult)
                    return;
                string result = args.Result;
                DowntimeNotificationFormModel downtimeDataResult = DialogFormHelper.Parse(result);
                if (downtimeDataResult != null)
                {
                    this.CreateUpdateItem(downtimeDataResult);
                }
                SheerResponse.Alert("Downtime Notification has been enabled");
            }
            else
            {
                UrlString urlString = new UrlString(UIUtil.GetUri("control:DowntimeNotification"));
                SheerResponse.ShowModalDialog(urlString.ToString(), "625px", "510px", string.Empty, true);
                args.WaitForPostBack();
            }
        }
        /// <summary>
        /// Creates/Updates the Notification Item
        /// </summary>
        /// <param name="downtimeDataResult">DowntimeNotificationFormModel</param>
        private void CreateUpdateItem(DowntimeNotificationFormModel downtimeDataResult)
        {
            Database db = Client.ContentDatabase;
            Item downtimeSettingsItem = db.GetItem(Constants.DownNotificationSettingsId);
            if (downtimeSettingsItem != null)
            {
                Item downtimeItem = downtimeSettingsItem.Children.Where(x => x.TemplateID == Templates.DowntimeNotificationItem.Id)?.FirstOrDefault();
                if (downtimeItem != null)
                {
                    ItemHelper.UpdateNotificationItem(downtimeItem, downtimeDataResult.Title, downtimeDataResult.StartTimeNotificationMessage, downtimeDataResult.EndTimeNotificationMessage, downtimeDataResult.StartTimeMaintenance, downtimeDataResult.EndTimeMaintenance, "1");
                }
                else
                {
                    ItemHelper.CreateNotificationItem(db, downtimeSettingsItem, "DownTimeNofication", downtimeDataResult.Title, downtimeDataResult.StartTimeNotificationMessage, downtimeDataResult.EndTimeNotificationMessage, downtimeDataResult.StartTimeMaintenance, downtimeDataResult.EndTimeMaintenance, "1");
                }
            }
        }
    }
}

