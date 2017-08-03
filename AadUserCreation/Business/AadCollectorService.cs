using AadUserCreation.Business.Interfaces;
using AadUserCreation.Data.Interfaces;
using AadUserCreation.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AadUserCreation.Business
{
    /// <summary>
    /// Collect and manage the storage of AAD users and groups in Blob.
    /// AAD Users and Groups are collected via the Graph API.
    /// </summary>
    public class AadCollectorService: IAadCollector
    {
        private readonly AppSettings _appSettings;
        private readonly IAadCollectieFromBlob _aadCollectieFromBlob;
        private readonly IAadCollectieFromAad _aadCollectieFromAad;

        public AadCollectorService(IOptions<AppSettings> appSettings, IAadCollectieFromAad aadCollectieFromAad, IAadCollectieFromBlob aadCollectieFromBlob)
        {
            _appSettings = appSettings.Value;
            _aadCollectieFromBlob = aadCollectieFromBlob;
            _aadCollectieFromAad = aadCollectieFromAad;
        }

        /// <summary>
        /// Load all groups and its members, first from Blob otherwise from AAD.
        /// 
        /// The Blob for storing AAD groups and users will bring some inconsistency between the App and AAD.
        /// THe tradeoff was speed and user experience. 
        /// Collecting every time the all AAD via Grpah API takes too much time for a proper user experience by large AD collections.
        /// A timerbased runbook can delete the Blob to restore the data, or the user can from the UI.
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<GroupListDetails> GetGroupMembershipsForGroups()
        {
            GroupListDetails groupsFrom;
            try
            {
                groupsFrom = await LoadGroupMembershipsForGroupsFromBlob();
                if (groupsFrom == null) 
                {
                    groupsFrom = await LoadGroupMembershipsForGroupsFromAad();
                }

                return groupsFrom;
            }
            catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get AAD data from AAD
        /// 
        /// Can take a while with big AAD's
        /// </summary>
        /// <returns></returns>
        public async Task<GroupListDetails> LoadGroupMembershipsForGroupsFromAad()
        {
            var result = await _aadCollectieFromAad.GetGroupListDetails();
            await _aadCollectieFromBlob.PutGroupListDetails(result);
            return result;
        }

        /// <summary>
        /// Load Groups and User data from Blob.
        /// 
        /// Can be inconsistent with AAD.
        /// </summary>
        /// <returns></returns>
        public async Task<GroupListDetails> LoadGroupMembershipsForGroupsFromBlob()
        {
            var result = await _aadCollectieFromBlob.GetGroupListDetails();
        
            return result;
        }

        /// <summary>
        /// Get last update time for the Blob.
        /// </summary>
        /// <returns></returns>
        public async Task<string> CollectionBlobState()
        {
            var state = await _aadCollectieFromBlob.GetCollectionBlobState();
            if (state == null)
                return "unkonws";
            else
            {
                var difference = DateTimeOffset.UtcNow - state;
                TimeSpan ts = (TimeSpan)difference;
                var hours = ts.Hours;
                if(hours == 0)
                {
                    return "Last refresh " + ts.Minutes + " minutes ago.";
                }
                return "Last refresh " + ts.Hours + " hours ago.";

            }
            
        }
    }
}
