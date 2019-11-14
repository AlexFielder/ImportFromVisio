using System;
using System.Collections.Generic;
using System.Text;

using Autodesk.Connectivity.WebServices;

namespace ImportFromVisio
{
    public class _UserInfo : UserInfo, IComparable
    {
        // a List is easier to deal with 
        public List<KnowledgeVault> _Vaults = new List<KnowledgeVault>();

        // permissions bits {read, write, delete}
        public bool[] _Permissions = new bool[] { false, false, false };

        // admin rights
        public bool _IsAdmin = false;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            UserInfo x = obj as UserInfo;

            if (x == null)
                return 1;

            return this.User.Name.CompareTo(x.User.Name);
        }

        #endregion

        public static _UserInfo[] Convert(UserInfo[] inputArray)
        {
            if (inputArray == null)
                return null;

            _UserInfo[] retval = new _UserInfo[inputArray.Length];

            for (int i = 0; i < inputArray.Length; i++)
            {
                retval[i] = _UserInfo.Convert(inputArray[i]);
            }

            return retval;
        }

        public static _UserInfo Convert(UserInfo inputObject)
        {
            if (inputObject == null)
                return null;

            return new _UserInfo()
            {
                Roles = inputObject.Roles,
                User = inputObject.User,
                Vaults = inputObject.Vaults
            };
        }
    }

    public class _GroupInfo : GroupInfo, IComparable
    {
        // a List is easier to deal with 
        public List<KnowledgeVault> _Vaults = new List<KnowledgeVault>();

        // permissions bits {read, write, delete}
        public bool[] _Permissions = new bool[] { false, false, false };

        // admin rights
        public bool _IsAdmin = false;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            GroupInfo x = obj as GroupInfo;

            if (x == null)
                return 1;

            return this.Group.Name.CompareTo(x.Group.Name);
        }

        #endregion

        public static _GroupInfo[] Convert(GroupInfo[] inputArray)
        {
            if (inputArray == null)
                return null;

            _GroupInfo[] retval = new _GroupInfo[inputArray.Length];

            for (int i = 0; i < inputArray.Length; i++)
            {
                retval[i] = _GroupInfo.Convert(inputArray[i]);
            }

            return retval;
        }

        public static _GroupInfo Convert(GroupInfo inputObject)
        {
            if (inputObject == null)
                return null;

            return new _GroupInfo()
            {
                Roles = inputObject.Roles,
                Group = inputObject.Group,
                Vaults = inputObject.Vaults
            };
        }
    }



    public class _Folder : Folder, IComparable
    {
        #region IComparable Members

        public int CompareTo(object obj)
        {
            Folder x = obj as Folder;

            if (x == null)
                return 1;

            return this.FullName.CompareTo(x.FullName);
        }

        #endregion

        public static _Folder Convert(Folder inputObject)
        {
            if (inputObject == null)
                return null;

            return new _Folder()
            {
                Cloaked = inputObject.Cloaked,
                CreateDate = inputObject.CreateDate,
                CreateUserId = inputObject.CreateUserId,
                CreateUserName = inputObject.CreateUserName,
                FullName = inputObject.FullName,
                FullUncName = inputObject.FullUncName,
                Id = inputObject.Id,
                //IsAct = inputObject.IsAct,
                IsLib = inputObject.IsLib,
                Locked = inputObject.Locked,
                Name = inputObject.Name,
                NumClds = inputObject.NumClds,
                ParId = inputObject.ParId
            };
        }
    }

    public class _ACE : ACE
    {
        /// <summary>
        /// A rewrite of the PermisArray property.
        /// Here we have the permission represented as an array of three integers.
        /// The first item in the array is Read access.  The second is Write.  The third is Delete.
        /// A value of 0 means no setting, a value of 1 means ALLOWED, a value of 2 means DENIED
        /// </summary>
        public int[] _Permissions;
    }
}
