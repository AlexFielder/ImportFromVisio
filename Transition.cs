using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Connectivity;
//using Autodesk.Connectivity.WebServices;

using ImportFromVisio.DocExSvc;
namespace ImportFromVisio
{
    /// <summary>
    /// A place to store details about each Transition
    /// </summary>
    class Transition
    {
        public string ToStateName;
        public string FromStateName;
        // these capture the 'reverse' of each transition.
        public string ReverseToStateName;
        public string ReverseFromStateName;

        public Transition(string fromStateName, string toStateName,string reverseToStateName,string reverseFromStateName)
        {
            this.FromStateName = fromStateName;
            this.ToStateName = toStateName;
            this.ReverseToStateName = reverseToStateName;
            this.ReverseFromStateName = reverseFromStateName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var trans = obj as Transition;

            if (obj == null)
                return false;

            return (this.FromStateName == trans.FromStateName && this.ToStateName == trans.ToStateName 
                && this.ReverseToStateName == trans.ReverseToStateName && this.ReverseFromStateName == trans.ReverseFromStateName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public long FromStateId
        {
            get; set;
        }
        public long ToStateId
        {
            get; set;
        }
        public bool EnforceChildStateSync
        {
            get; set;
        }
        public BumpRevisionEnum BumpRevision
        { 
            get; set; 
        }
        public JobSyncPropEnum SyncPropOption
        {
            get; set;
        }
        public bool UseTransitionBasedSecurity
        {
            get; set;
        }
        //public long[] AllowedUserIds = new long[50]; //50 because we don't have that many users!
        public List<long> AllowedUserIds = new List<long>();
        //public long[] DeniedUserIds = new long[50]; //50 because we don't have that many users!
        public List<long> DeniedUserIds = new List<long>();
        public PropDefCond[] Conditions
        {
            get; set;
        }
    }
}
