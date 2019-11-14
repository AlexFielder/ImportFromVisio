using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.Visio;

using AccessPermis = ImportFromVisio.DocExSvc.AccessPermis;
using ACE = ImportFromVisio.DocExSvc.ACE;
using BumpRevisionEnum = ImportFromVisio.DocExSvc.BumpRevisionEnum;
using JobSyncPropEnum = ImportFromVisio.DocExSvc.JobSyncPropEnum;
using LfCycDef = ImportFromVisio.DocExSvc.LfCycDef;
using LfCycState = ImportFromVisio.DocExSvc.LfCycState;
using RestrictPurgeOption = ImportFromVisio.DocExSvc.RestrictPurgeOption;

namespace ImportFromVisio
{
    public partial class MainForm : Form
    {
        private List<string> m_lcStates;
        private List<Pair> m_lcTrans;

        //private static List<State> _statesList = new List<State>();
        private List<State> _statesList;
        //private static List<Transition> _transitionsList = new List<Transition>();
        private List<Transition> _transitionsList;
        private List<string> _shapetextlist;

        public MainForm()
        {
            InitializeComponent();
        }

        private void m_visioFileButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Visio Files (*.vsd)|*.vsd";
            fileDialog.FileName = m_visioFileTextBox.Text;

            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_visioFileTextBox.Text = fileDialog.FileName;
            }
        }

        private void m_visioFileTextBox_TextChanged(object sender, EventArgs e)
        {
            Enabled = false;
            if (!LoadVisioFileAlex()) return;
            SetStatedataAlex();
            //LoadVisioFile();
            //SetStatedata();
            Enabled = true;
        }
        /// <summary>
        /// Sets the Default state based on the _statesList object
        /// </summary>
        private void SetStatedataAlex()
        {
            if(_statesList == null)
            {
                return;
            }
            m_defaultStateComboBox.Items.Clear();
            m_defaultStateComboBox.Items.AddRange(_shapetextlist.ToArray());
            m_defaultStateComboBox.SelectedIndex = 0;
        }

        private void SetStatedata()
        {
            if (m_lcStates == null)
                return;

            m_defaultStateComboBox.Items.Clear();
            m_defaultStateComboBox.Items.AddRange(m_lcStates.ToArray());
            m_defaultStateComboBox.SelectedIndex = 0;
        }
        private const string Str = "true";
        //Process data constants:
        private const string DisplayName = "Row_1";
        private const string Description = "Row_2";
        private const string IsDefault = "Row_3";
        private const string UseStateBasedSecurity = "Row_4";
        private const string AccessControlEntryPermissions = "Row_5";
        private const string AccessControlEntryUsers = "Row_6";
        private const string Comments = "Row_7";
        private const string IsReleasedState = "Row_8";
        private const string MyRestrictPurgeOption = "Row_9";

        //Connector data constants:
        private const string FromStateId = "Row_1";
        private const string ToStateId = "Row_2";
        private const string EnforceChildStateSync = "Row_3";
        private const string BumpRevision = "Row_4";
        private const string SyncPropOption = "Row_5";
        private const string UseTransitionBasedSecurity = "Row_6";
        private const string Conditions = "Row_9";
        //connector string[] arrays:
        private const string AllowedUserIds = "Row_7";
        private const string DeniedUserIds = "Row_8";
        /// <summary>
        /// Loads a Visio file, reads values from the 'States' and 'Transitions' contained within.
        /// </summary>
        /// <returns>Returns true if no errors were encountered.</returns>
        private bool LoadVisioFileAlex()
        {
            bool returnval = true;
            string filepath = m_visioFileTextBox.Text;
            Microsoft.Office.Interop.Visio.Application visio = null;
            Document doc = null;
            //try
            //{
                if (!filepath.EndsWith(".vsd") || !System.IO.File.Exists(filepath))
                    return false;
                _statesList = new List<State>();
                _transitionsList = new List<Transition>();
                _shapetextlist = new List<string>();
                m_lcTrans = new List<Pair>();
                State _State;
                var _Trans = new Transition(null, null,null,null);
                var newACEs = new Dictionary<long, _ACE>();
                //Cell customPropertyCell = null;
                //string teststr = null;

                visio = new Microsoft.Office.Interop.Visio.Application();
                doc = visio.Documents.Open(filepath);
                var page = doc.Pages[1];

                var connectorMap = new Dictionary<string, ConnectorInfo>();

                foreach (Shape shape in page.Shapes)
                {

                    if (shape.Name.StartsWith("Admin") || shape.Name.StartsWith("QA") || shape.Name.StartsWith("Design"))
                    {
                        _State = new State();
                        _State.Statename = shape.Text;
                        //_State.Statename = ReadANamedCustomProperty(shape, DisplayName, true);
                        if (_State.Statename == null)
                        {
                            continue;
                        }
                        if (_shapetextlist.Contains(shape.Text))
                        {
                            throw new Exception("Each state name must be unique.  " +
                                                "State " + shape.Text + " shows up multiple times.");
                        }

                        //Users allowed to have this permission in each state.
                        var tmpACEUsers = ReadANamedCustomProperty(shape, AccessControlEntryUsers,true);
                        if (tmpACEUsers.Contains(","))
                        {
                            var tmpACEusers = tmpACEUsers.Split(',');
                            foreach (var user in tmpACEusers)
                            {
                                _State.AccessControlEntryUserGroups.Add(Convert.ToInt64(user));
                            }
                        }
                        else
                        {
                            _State.AccessControlEntryUserGroups.Add(Convert.ToInt64(tmpACEUsers));
                        }
                        // Permissions for each State.
                        var tmpACEPermissions = ReadANamedCustomProperty(shape,AccessControlEntryPermissions,true);
                        _State.AccessControlEntry = AssignACEPermissions(_State.Statename, tmpACEPermissions,
                                                                         _State.AccessControlEntryUserGroups);
                        var comments = ReadANamedCustomProperty(shape, Comments, true);
                        if (comments.Contains(";"))
                        {
                            var tmpcomments = comments.Split(';');
                            _State.Comments.Add(Convert.ToString(tmpcomments));
                        }
                        else
                        {
                            _State.Comments.Add(comments);
                        }
                        //_State.Comments = ReadANamedCustomProp(shape, _comments, true);
                        if (ReadANamedCustomProperty(shape, IsDefault, true).ToUpper() == Str.ToUpper())
                        {
                            _State.IsDefault = true;
                        }
                        else
                        {
                            _State.IsDefault = false;
                        }
                        if (ReadANamedCustomProperty(shape, IsReleasedState, true).ToUpper() == Str.ToUpper())
                        {
                            _State.IsReleasedState = true;
                        }
                        else
                        {
                            _State.IsReleasedState = false;
                        }
                        switch (ReadANamedCustomProperty(shape, MyRestrictPurgeOption, true))
                        {
                            case ".All":
                                _State.RestrictPurgeOption = RestrictPurgeOption.All;
                                break;
                            case ".FirstAndLast":
                                _State.RestrictPurgeOption = RestrictPurgeOption.FirstAndLast;
                                break;
                            case ".Last":
                                _State.RestrictPurgeOption = RestrictPurgeOption.Last;
                                break;
                            case ".None":
                                _State.RestrictPurgeOption = RestrictPurgeOption.None;
                                break;
                        }
                        _State.StateDescription = ReadANamedCustomProperty(shape, Description, true);
                        if (ReadANamedCustomProperty(shape, UseStateBasedSecurity, true).ToUpper() == Str.ToUpper())
                        {
                            _State.UseStateBasedSecurity = true;
                        }
                        else
                        {
                            _State.UseStateBasedSecurity = false;
                        }
                        if (shape.Text != null)
                        {
                            _shapetextlist.Add(shape.Text);
                            _statesList.Add(_State);
                        }
                    }
                    else if (shape.Name.StartsWith("Dynamic connector"))
                    {
                        _Trans = new Transition(null,null,null,null);
                        foreach (Microsoft.Office.Interop.Visio.Connect connect in shape.Connects)
                        {
                            var fromtext = connect.FromCell.Shape.Text;
                            var fromname = connect.FromCell.Shape.Name;
                            var totext = connect.ToCell.Shape.Text;
                            _Trans.FromStateName = fromtext;
                            _Trans.ToStateName = totext;
                            _Trans.ReverseFromStateName = totext;
                            _Trans.ReverseToStateName = fromtext;
                            var tmpallowedusers = ReadANamedCustomProperty(shape, AllowedUserIds, true);
                            if (tmpallowedusers.Contains(","))
                            {
                                var tmpAllowedUsers = tmpallowedusers.Split(',');
                                foreach (var tmpstr in tmpAllowedUsers)
                                {
                                    _Trans.AllowedUserIds.Add(Convert.ToInt64(tmpstr));
                                }
                            }
                            else
                            {
                                _Trans.AllowedUserIds.Add(Convert.ToInt64(tmpallowedusers));
                            }
                            var tmpdeniedusers = ReadANamedCustomProperty(shape, DeniedUserIds, true);
                            if (tmpdeniedusers.Contains(","))
                            {
                                var tmpDeniedUsers = tmpdeniedusers.Split(',');
                                foreach (var tmpstr in tmpDeniedUsers)
                                {
                                    _Trans.DeniedUserIds.Add(Convert.ToInt64(tmpstr));
                                }
                            }
                            else
                            {
                                _Trans.DeniedUserIds.Add(Convert.ToInt64(tmpdeniedusers));
                            }
                            switch(ReadANamedCustomProperty(shape, SyncPropOption, true))
                            {
                                case "None":
                                    _Trans.SyncPropOption = JobSyncPropEnum.None;
                                    break;
                                case "SysncPropAndUpdateView":
                                    _Trans.SyncPropOption = JobSyncPropEnum.SyncPropAndUpdateView;
                                    break;
                                case "SyncPropOnly":
                                    _Trans.SyncPropOption = JobSyncPropEnum.SyncPropOnly;
                                    break;
                            }
                            if (ReadANamedCustomProperty(shape, EnforceChildStateSync, true).ToUpper() == Str.ToUpper())
                            {
                                _Trans.EnforceChildStateSync = true;
                            }
                            else
                            {
                                _Trans.EnforceChildStateSync = false;
                            }
                            if (ReadANamedCustomProperty(shape, UseTransitionBasedSecurity, true).ToUpper() == Str.ToUpper())
                            {
                                _Trans.UseTransitionBasedSecurity = true;
                            }
                            else
                            {
                                _Trans.UseTransitionBasedSecurity = false;
                            }
                            switch(ReadANamedCustomProperty(shape, BumpRevision, true))
                            {
                                case ".BumpPrimary":
                                    _Trans.BumpRevision = BumpRevisionEnum.BumpPrimary;
                                    break;
                                case ".BumpSecondary":
                                    _Trans.BumpRevision = BumpRevisionEnum.BumpSecondary;
                                    break;
                                case".BumpTertiary":
                                    _Trans.BumpRevision = BumpRevisionEnum.BumpTertiary;
                                    break;
                                case".None":
                                    _Trans.BumpRevision = BumpRevisionEnum.None;
                                    break;
                            }

                            var endarrow = connect.FromSheet.Cells["endarrow"];
                            var beginarrow = connect.FromSheet.Cells["beginarrow"];

                            ConnectorInfo info = null;
                            if (connectorMap.ContainsKey(fromname))
                                info = connectorMap[fromname];
                            else
                            {
                                info = new ConnectorInfo(fromname);
                                connectorMap.Add(fromname, info);
                            }
                            var hasarrow = !beginarrow.Formula.StartsWith("theme");
                            info.AddConnection(totext, hasarrow);
                            if (info.ShapeNames.Count > 1)
                            {
                                if (info.ShapeNames[1] == _Trans.ToStateName)
                                {
                                    _Trans.FromStateName = info.ShapeNames[0];
                                    _Trans.ReverseToStateName = _Trans.FromStateName;
                                    _transitionsList.Add(_Trans);
                                }
                            }
                        }
                    }
                }

                Dictionary<string, ConnectorInfo>.Enumerator enumerator = connectorMap.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ConnectorInfo info = enumerator.Current.Value;

                    if (info.ShapeNames.Count != 2)
                        continue;

                    // the default connector
                    if (!info.HasArrows[0] && !info.HasArrows[1])
                        AddPair(new Pair(info.ShapeNames[0], info.ShapeNames[1]));

                    if (info.HasArrows[0])
                        AddPair(new Pair(info.ShapeNames[0], info.ShapeNames[1]));

                    // the arrow goes the other way too
                    if (info.HasArrows[1])
                        AddPair(new Pair(info.ShapeNames[1], info.ShapeNames[0]));
                }
            //}
            //catch (Exception e)
            //{
            //    m_lcStates = null;
            //    m_lcTrans = null;
            //    MessageBox.Show("Exception thrown: " + e.Message);
            //    returnval = false;
            //}
            //finally
            //{
                if (doc != null)
                    doc.Close();

                if (visio != null)
                    visio.Quit();
            //}
            return returnval;
        }
        /// <summary>
        /// Assigns permissions in the tmpACEPermissions field to our ACE[] Array
        /// </summary>
        /// <param name="statename">The State affected</param>
        /// <param name="tmpACEPermissions">a list of permissions granted</param>
        /// <param name="users">the users affected</param>
        /// <returns>Returns an array of Access Control Entry Permissions</returns>
        private static ACE[] AssignACEPermissions(string statename,string tmpACEPermissions, List<long> users)
        {
            var modifyPermis = new AccessPermis();
            var readPermis = new AccessPermis();
            var deletePermis = new AccessPermis();
            var aceList = new List<ACE>{};
            switch (tmpACEPermissions)
            {
                case "AllowWrite":
                    readPermis = new AccessPermis { Id = 1, Val = true };
                    modifyPermis = new AccessPermis { Id = 2, Val = true };
                    deletePermis = new AccessPermis { Id = 3, Val = false };
                    break;
                case "AllowDelete":
                    if (!statename.EndsWith("Copy In"))
                    {
                        throw new Exception("AllowDelete permission is only allowed at 'Paper/Digital Copy In' States!");
                    }
                    if (statename == "Digital Copy In" | statename == "Paper Copy In")
                    {
                        readPermis = new AccessPermis { Id = 1, Val = true };
                        modifyPermis = new AccessPermis { Id = 2, Val = true };
                        deletePermis = new AccessPermis { Id = 3, Val = true };//this is only allowed for Admin roles at job start!
                    }
                    break;
                default:
                    readPermis = new AccessPermis { Id = 1, Val = true };
                    modifyPermis = new AccessPermis { Id = 2, Val = false };
                    deletePermis = new AccessPermis { Id = 3, Val = false };
                    break;
            }
            foreach (var user in users)
            {
                if (user == 0) continue;
                var aceA = default(ACE);
                aceA = new ACE { UserGrpId = user };
                var permisList = new List<AccessPermis> { readPermis, modifyPermis, deletePermis };
                aceA.PermisArray = permisList.ToArray();
                aceList.Add(aceA);//add users here as appropriate var aceList = new List<ACE> {aceA,aceB,aceC,aceD}; etc.
            }
            return aceList.ToArray();
        }

        /// <summary>
        ///   Reads the Custom Property of a Visio Shape.
        /// </summary>
        /// <param name = "customPropertyShape">the shape we're reading data from.</param>
        /// <param name = "cellName">the name of the property we're looking for.</param>
        /// <param name = "isLocalName">is the property name local?</param>
        /// <returns>returns a string value for the associated property.</returns>
        private static string ReadANamedCustomProperty(
            Shape customPropertyShape,
            string cellName, bool isLocalName)
        {
            if (customPropertyShape == null || cellName == null)
            {
                return null;
            }

            const string CUST_PROP_PREFIX = "Prop.";

            Microsoft.Office.Interop.Visio.Application visioApplication =
                customPropertyShape.Application;

            string propName;
            Cell customPropertyCell = null;

            try
            {
                // Verify that all incoming string parameters are not of zero 
                // length, except for the ones that have default values as ""
                // and the output parameters.
                if (cellName.Length == 0)
                {
                    throw new System.ArgumentNullException("cellName",
                                                           "Zero length string input.");
                }

                // Custom properties have a prefix of "Prop".
                propName = CUST_PROP_PREFIX + cellName;

                // If the custom property exists, get the value of the cell from
                // the shape.
                if (isLocalName)
                {
                    
                    if (customPropertyShape.get_CellExists(propName,
                                                           (short) VisExistsFlags.
                                                                       visExistsAnywhere) != 0)
                    {
                        // If the cell exists, the cell variable gets a
                        // valid value
                        customPropertyCell = customPropertyShape.get_Cells(
                            propName);
                    }
                }
                else if (customPropertyShape.get_CellExistsU(propName,
                                                             (short) VisExistsFlags.
                                                                         visExistsAnywhere) != 0)
                {
                    // If the cell exists, the cell variable gets a
                    // valid value
                    customPropertyCell = customPropertyShape.get_CellsU(
                        propName);
                }

                // Only display message boxes if the AlertResponse is 0.
                if (visioApplication.AlertResponse == 0)
                {
                    // Proceed if the property exists.
                    if (customPropertyCell == null)
                    {
                        //System.Windows.Forms.MessageBox.Show(propName +
                        //    " was not found.");
                        return null;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                throw;
            }
            //return propName + ".ResultStr = " + customPropertyCell.get_ResultStr(Microsoft.Office.Interop.Visio.VisUnitCodes.visNoCast);
            return customPropertyCell.get_ResultStr(Microsoft.Office.Interop.Visio.VisUnitCodes.visNoCast);
        }

        private void LoadVisioFile()
        {
            var filepath = m_visioFileTextBox.Text;

            Microsoft.Office.Interop.Visio.Application visio = null;
            Microsoft.Office.Interop.Visio.Document doc = null;
            try
            {
                if (!filepath.EndsWith(".vsd") || !System.IO.File.Exists(filepath))
                    return;

                m_lcStates = new List<string>();
                m_lcTrans = new List<Pair>();

                visio = new Microsoft.Office.Interop.Visio.Application();
                doc = visio.Documents.Open(filepath);
                var page = doc.Pages[1];

                var connectorMap = new Dictionary<string, ConnectorInfo>();

                foreach (Microsoft.Office.Interop.Visio.Shape shape in page.Shapes)
                {
                    if (shape.Name.StartsWith("Process"))
                    {
                        if (m_lcStates.Contains(shape.Text))
                        {
                            throw new Exception("Each state name must be unique.  " +
                                                "State " + shape.Text + " shows up multiple times.");
                        }

                        m_lcStates.Add(shape.Text);
                    }
                    else if (shape.Name.StartsWith("Dynamic connector"))
                    {
                        foreach (Connect connect in shape.Connects)
                        {
                            string fromName = connect.FromCell.Shape.Name;
                            string toText = connect.ToCell.Shape.Text;

                            Cell endArrow = connect.FromSheet.get_Cells("EndArrow");
                            Cell beginArrow = connect.FromSheet.get_Cells("BeginArrow");

                            ConnectorInfo info = null;
                            if (connectorMap.ContainsKey(fromName))
                                info = connectorMap[fromName];
                            else
                            {
                                info = new ConnectorInfo(fromName);
                                connectorMap.Add(fromName, info);
                            }
                            bool hasArrow = !beginArrow.Formula.StartsWith("THEME");
                            info.AddConnection(toText, hasArrow);
                        }
                    }
                }

                Dictionary<string, ConnectorInfo>.Enumerator enumerator = connectorMap.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ConnectorInfo info = enumerator.Current.Value;

                    if (info.ShapeNames.Count != 2)
                        continue;

                    // the default connector
                    if (!info.HasArrows[0] && !info.HasArrows[1])
                        AddPair(new Pair(info.ShapeNames[0], info.ShapeNames[1]));

                    if (info.HasArrows[0])
                        AddPair(new Pair(info.ShapeNames[0], info.ShapeNames[1]));

                    // the arrow goes the other way too
                    if (info.HasArrows[1])
                        AddPair(new Pair(info.ShapeNames[1], info.ShapeNames[0]));
                }
            }
            catch (Exception e)
            {
                m_lcStates = null;
                m_lcTrans = null;
                MessageBox.Show("Exception thrown: " + e.Message);
            }
            finally
            {
                if (doc != null)
                    doc.Close();

                if (visio != null)
                    visio.Quit();
            }
        }

        private void AddPair(Pair pair)
        {
            if (!m_lcTrans.Contains(pair))
                m_lcTrans.Add(pair);
        }

        private void m_createButton_Click(object sender, EventArgs e)
        {
            Enabled = false;
            //CreateLifecyleDefinition();
            CreateLifecycleDefinitionAlex();
            Enabled = true;
        }

        private void CreateLifecycleDefinitionAlex()
        {
            if (_statesList == null || _transitionsList == null)
            {
                MessageBox.Show("You must select load a Visio file");
                return;
            }
            var defaultState = (string)m_defaultStateComboBox.SelectedItem;
            if (defaultState.Length == 0)
            {
                MessageBox.Show("You must select a default state");
                return;
            }

            var lcDefName = m_lcDefNameTextBox.Text;
            if (lcDefName.Length == 0)
            {
                MessageBox.Show("You must enter a lifecycle definition name");
                return;
            }

            var lcDefDesc = m_lcDescTextBox.Text;

            try
            {

                var docExSvc = ServiceManager.GetDocumentServiceExtensions();

                var lcDef = docExSvc.AddLifeCycleDefinition(lcDefName, lcDefDesc);

                // map the state name to the state object
                var mStateObjMap = new Dictionary<string, LfCycState>();

                foreach (var lcStateName in _shapetextlist)
                {
                    var state = (from st in _statesList
                                  where st.Statename == lcStateName
                                  select st).FirstOrDefault();
                    var isDefault = state.IsDefault;
                    var comments = new string[state.Comments.Count];
                    var i = 0;
                    foreach (var comment in state.Comments)
                    {
                        comments[i] = comment;
                        i++;
                    }
                    var lcStateObj = docExSvc.AddLifeCycleState(lcDef.Id, lcStateName, state.StateDescription,
                                                                isDefault, state.UseStateBasedSecurity,state.AccessControlEntry,
                                                                comments, state.IsReleasedState,
                                                                state.RestrictPurgeOption);
                    mStateObjMap.Add(lcStateName, lcStateObj);
                }

                foreach (var trans in m_lcTrans)
                {
                    var transition = (from tr in _transitionsList
                                      where tr.FromStateName == trans.FromStateName && tr.ToStateName == trans.ToStateName
                                      select tr).FirstOrDefault() ?? (from tr in _transitionsList
                                                                      where
                                                                          tr.ReverseFromStateName == trans.FromStateName &&
                                                                          tr.ReverseToStateName == trans.ToStateName
                                                                      select tr).FirstOrDefault();
                    if(transition == null)
                        throw new Exception("There is no corresponding transition in this workflow");
                    var fromId = mStateObjMap[trans.FromStateName].Id;
                    var toId = mStateObjMap[trans.ToStateName].Id;
                    var tmpallowedlist = new long[transition.AllowedUserIds.Count];
                    var i = 0;
                    foreach (var user in transition.AllowedUserIds)
                    {
                        tmpallowedlist[i] = user;
                        i++;
                    }
                    var tmpdeniedlist = new long[transition.DeniedUserIds.Count];
                    var j = 0;
                    foreach (var user in transition.DeniedUserIds)
                    {
                        tmpdeniedlist[j] = user;
                        j++;
                    }
                    docExSvc.AddLifeCycleStateTransition(fromId, toId, transition.EnforceChildStateSync,
                                                         transition.BumpRevision, transition.SyncPropOption,
                                                         transition.UseTransitionBasedSecurity,
                                                         tmpallowedlist, tmpdeniedlist,
                                                         transition.Conditions);
                    //docExSvc.AddLifeCycleStateTransition(fromId, toId, false, BumpRevisionEnum.None,
                    //                                     JobSyncPropEnum.None, false, null, null, null);
                }

                MessageBox.Show("Import Completed");
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception thrown: " + e.Message);
            }
        }

        /// <summary>
        ///   Connect to Vault and add all the lifecycle information
        /// </summary>
        private void CreateLifecyleDefinition()
        {
            if (m_lcStates == null || m_lcTrans == null)
            {
                MessageBox.Show("You must select load a Visio file");
                return;
            }

            var defaultState = (string) m_defaultStateComboBox.SelectedItem;
            if (defaultState.Length == 0)
            {
                MessageBox.Show("You must select a default state");
                return;
            }

            string lcDefName = m_lcDefNameTextBox.Text;
            if (lcDefName.Length == 0)
            {
                MessageBox.Show("You must enter a lifecycle definition name");
                return;
            }

            string lcDefDesc = m_lcDescTextBox.Text;

            try
            {
                DocExSvc.DocumentServiceExtensions docExSvc = ServiceManager.GetDocumentServiceExtensions();

                LfCycDef lcDef = docExSvc.AddLifeCycleDefinition(lcDefName, lcDefDesc);

                // map the state name to the state object
                var m_stateObjMap = new Dictionary<string, LfCycState>();

                foreach (string lcStateName in m_lcStates)
                {
                    bool isDefault = (lcStateName == defaultState);
                    LfCycState lcStateObj = docExSvc.AddLifeCycleState(lcDef.Id, lcStateName, "", isDefault, false,
                                                                       null, null, false, RestrictPurgeOption.None);
                    m_stateObjMap.Add(lcStateName, lcStateObj);
                }

                foreach (Pair trans in m_lcTrans)
                {
                    long fromId = m_stateObjMap[trans.FromStateName].Id;
                    long toId = m_stateObjMap[trans.ToStateName].Id;

                    docExSvc.AddLifeCycleStateTransition(fromId, toId, false, BumpRevisionEnum.None,
                                                         JobSyncPropEnum.None, false, null, null, null);
                }

                MessageBox.Show("Import Completed");
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception thrown: " + e.Message);
            }
        }
    }

    /// <summary>
    ///   A place to hold the TO and FROM states
    /// </summary>
    internal class Pair
    {
        public string ToStateName;
        public string FromStateName;

        public Pair(string fromStateName, string toStateName)
        {
            FromStateName = fromStateName;
            ToStateName = toStateName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var pair = obj as Pair;

            if (obj == null)
                return false;

            return (FromStateName == pair.FromStateName && ToStateName == pair.ToStateName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    /// <summary>
    ///   A place to store information from a Visio connector.
    /// </summary>
    internal class ConnectorInfo
    {
        private string m_name;
        private List<bool> m_hasArrows;
        private List<string> m_shapeNames;

        public ConnectorInfo(string name)
        {
            m_name = name;
            m_hasArrows = new List<bool>();
            m_shapeNames = new List<string>();
        }

        public void AddConnection(string name, bool hasArrow)
        {
            m_shapeNames.Add(name);
            m_hasArrows.Add(hasArrow);
        }

        public List<string> ShapeNames
        {
            get { return m_shapeNames; }
        }

        public List<bool> HasArrows
        {
            get { return m_hasArrows; }
        }
    }
}