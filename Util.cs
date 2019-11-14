using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Autodesk.Connectivity.WebServices;

namespace ImportFromVisio
{
    public enum PermissionState
    {
        NO_ACCESS,
        READ_ONLY,
        READ_WRITE,
        FULL_CONTROL
    }

    /// <summary>
    /// Various utility functions
    /// </summary>
    public class Util
    {
        //public static Icon NO_ACCESS_ICON = Properties.Resources.NoAccess;
        //public static Icon READ_ICON = Properties.Resources.ReadOnly;
        //public static Icon READ_WRITE_ICON = Properties.Resources.ReadWrite;
        //public static Icon FULL_ACCESS_ICON = Properties.Resources.FullControl;

        public static int GetDepth(Folder folder)
        {
            System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex("/");
            return regx.Matches(folder.FullName).Count;
        }

        public static void AddItemToList(long item, List<long> list)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void AddListToList(List<long> subList, List<long> parentList)
        {
            foreach (long item in subList)
            {
                AddItemToList(item, parentList);
            }
        }

        public static void AddAceToMap(_ACE ace, Dictionary<long, _ACE> map)
        {
            if (!map.ContainsKey(ace.UserGrpId))
                map.Add(ace.UserGrpId, ace);
            else
            {
                // merge the new ACE with the existing one
                _ACE ace2 = map[ace.UserGrpId];

                for (int i = 0; i < ace2._Permissions.Length; i++)
                {
                    // overwrite the permission if the number is higher
                    // "allow" is higher than "not set" and "deny" is higher than "allow"
                    if (ace._Permissions[i] > ace2._Permissions[i])
                        ace2._Permissions[i] = ace._Permissions[i];
                }
            }
        }

        public static void AddPermissionsToList(List<long> list, Permis[] permissions)
        {
            if (permissions == null)
                return;

            foreach (Permis permission in permissions)
            {
                AddItemToList(permission.Id, list);
            }
        }

        /// <summary>
        /// Convert AccessPermis[] into a fixed array of 3 integers.
        /// </summary>
        public static int[] ConvertPermissions(AccessPermis[] permissions)
        {
            // initalize the array to zeros, which means the bits are not set
            int[] retVal = new int[] { 0, 0, 0 };

            if (permissions != null && permissions.Length > 0)
            {
                foreach (AccessPermis permission in permissions)
                {
                    if (permission.Val)
                        retVal[permission.Id - 1] = 1;  // the bit is set to allow
                    else
                        retVal[permission.Id - 1] = 2;  // the bit is set to deny
                }
            }

            return retVal;
        }

        public static PermissionState DecodePermissions(_UserInfo user)
        {
            return DecodePermissions(new int[] { 1, 1, 1 }, user);
        }

        public static PermissionState DecodePermissions(_GroupInfo group)
        {
            return DecodePermissions(new int[] { 1, 1, 1 }, group);
        }

        public static PermissionState DecodePermissions(int[] permissions, _UserInfo user)
        {
            // a value of 1 means that access has been granted.
            // a value of 0 (not set) or 2 (denied) results i no access.

            // we assume no read also means no write or delete
            if (permissions[0] != 1 || !user._Permissions[0])
            {
                if (user._IsAdmin)
                    return PermissionState.READ_ONLY;   // administrators can always read.
                else
                    return PermissionState.NO_ACCESS;
            }

            // read is granted but not write.  Assume delete is also not granted
            if (permissions[1] != 1 || !user._Permissions[1])
                return PermissionState.READ_ONLY;

            // read and write granted, but not delete
            if (permissions[2] != 1 || !user._Permissions[2])
                return PermissionState.READ_WRITE;

            // if we got this far, it means everything was granted
            return PermissionState.FULL_CONTROL;
        }

        public static PermissionState DecodePermissions(int[] permissions, _GroupInfo group)
        {
            // a value of 1 means that access has been granted.
            // a value of 0 (not set) or 2 (denied) results i no access.

            // we assume no read also means no write or delete
            if (permissions[0] != 1 || !group._Permissions[0])
            {
                if (group._IsAdmin)
                    return PermissionState.READ_ONLY;   // administrators can always read.
                else
                    return PermissionState.NO_ACCESS;
            }

            // read is granted but not write.  Assume delete is also not granted
            if (permissions[1] != 1 || !group._Permissions[1])
                return PermissionState.READ_ONLY;

            // read and write granted, but not delete
            if (permissions[2] != 1 || !group._Permissions[2])
                return PermissionState.READ_WRITE;

            // if we got this far, it means everything was granted
            return PermissionState.FULL_CONTROL;
        }


        //public static object GetGridValue(PermissionState state)
        //{
        //    if (state == PermissionState.FULL_CONTROL)
        //        return FULL_ACCESS_ICON;
        //    else if (state == PermissionState.NO_ACCESS)
        //        return NO_ACCESS_ICON;
        //    else if (state == PermissionState.READ_ONLY)
        //        return READ_ICON;
        //    else if (state == PermissionState.READ_WRITE)
        //        return READ_WRITE_ICON;
        //    else
        //        throw new Exception("Unknown permission state");
        //}

        public static void SetUserPermissions(_UserInfo user, List<long> permissions)
        {
            if (permissions == null)
                return;

            foreach (long permission in permissions)
            {
                if (permission == 6) // file read
                    user._Permissions[0] = true;
                else if (permission == 8) // file check out
                    user._Permissions[1] = true;
                else if (permission == 5) // file delete conditional
                    user._Permissions[2] = true;
                else if (permission == 81) // admin user write
                    user._IsAdmin = true;
            }
        }

        public static void SetUserPermissions(_GroupInfo group, List<long> permissions)
        {
            if (permissions == null)
                return;

            foreach (long permission in permissions)
            {
                if (permission == 6) // file read
                    group._Permissions[0] = true;
                else if (permission == 8) // file check out
                    group._Permissions[1] = true;
                else if (permission == 5) // file delete conditional
                    group._Permissions[2] = true;
                else if (permission == 81) // admin user write
                    group._IsAdmin = true;
            }
        }

        public static void SetGroupPermissions(_GroupInfo group, List<long> permissions)
        {
            if (permissions == null)
                return;

            foreach (long permission in permissions)
            {
                if (permission == 6) // file read
                    group._Permissions[0] = true;
                else if (permission == 8) // file check out
                    group._Permissions[1] = true;
                else if (permission == 5) // file delete conditional
                    group._Permissions[2] = true;
                else if (permission == 81) // admin user write
                    group._IsAdmin = true;
            }
        }



    }
}
