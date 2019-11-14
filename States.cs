
using System;
using System.Collections.Generic;
using System.Text;
//using Autodesk.Connectivity.WebServices;

using ImportFromVisio.DocExSvc;

namespace ImportFromVisio
{
    class State
    {
        public string Statename
        {
            get; set;
        }
        public string StateDescription
        {
            get; set;
        }
        public bool IsDefault
        {
            get; set;
        }
        public bool UseStateBasedSecurity
        {
            get; set;
        }
        //public long[] AccessControlEntryPermissions = new long[50];
        public List<long> AccessControlEntryPermissions = new List<long>();
        //public long[] AccessControlEntryUserGroups = new long[50];
        public List<long> AccessControlEntryUserGroups = new List<long>();
        //public long[] AllowedUserIds = new long[50]; //50 because we don't have that many users!
        public List<long> AllowedUserIds = new List<long>();
        //public long[] DeniedUserIds = new long[50]; //50 because we don't have that many users!
        public List<long> DeniedUserIds = new List<long>();

        //public string[] Comments = new string[50]; //50 because there won't be that many comments!
        public List<string> Comments = new List<string>();
        public bool IsReleasedState
        {
            get; set;
        }
        public RestrictPurgeOption RestrictPurgeOption
        {
            get; set;
        }
        public ACE[] AccessControlEntry
        {
            get; set;
        }
    }
}
