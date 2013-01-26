using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using IniParser;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    public class TeamPilgrimSettings
    {
        private const string GeneralSectionName = "General";
        private const string SelectedWorkItemCheckinActionKeyName = "SelectedWorkItemCheckinAction";
        private const string PreserveShelvedChangesLocallyKeyName = "PreserveShelvedChangesLocally";
        private const string EvaluatePoliciesDuringShelveKeyName = "EvaluatePoliciesDuringShelve";
        private const string PreviouslySelectedWorkItemQueriesValueSeperatorKeyName = "PreviouslySelectedWorkItemQueriesValueSeperator";
        private const string PreviouslySelectedWorkItemQueriesMaxCountKeyName = "PreviouslySelectedWorkItemQueriesMaxCount";

        private const string ShelvingSectionName = "Shelving";

        private const string PreviouslySelectedWorkItemQueriesSectionName = "PreviouslySelectedWorkItemQueries";

        static TeamPilgrimSettings()
        {
            var directoryName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Debug.Assert(directoryName != null, "directoryName != null");

            GetSettingsFilePath = Path.Combine(directoryName, "TeamPilgrim", @"teampilgrim.ini");
        }

        private static readonly string GetSettingsFilePath;

        private readonly IniData _iniData;
        private readonly FileIniDataParser _parser;

        public TeamPilgrimSettings()
        {
            _parser = new FileIniDataParser();

            try
            {
                _iniData = _parser.LoadFile(GetSettingsFilePath);
            }
            catch (Exception ex)
            {
                _iniData = new IniData();
                SelectedWorkItemCheckinAction = SelectedWorkItemCheckinAction;
                PreviouslySelectedWorkItemQueriesValueSeperator = PreviouslySelectedWorkItemQueriesValueSeperator;
                PreviouslySelectedWorkItemQueriesMaxCount = PreviouslySelectedWorkItemQueriesMaxCount;
                PreserveShelvedChangesLocally = PreserveShelvedChangesLocally;
                EvaluatePoliciesDuringShelve = EvaluatePoliciesDuringShelve;
                Save();
            }

            PreviouslySelectedWorkItemsQueries = new PreviouslySelectedWorkItemsQueriesCollection(this);
        }

        #region General

        private KeyDataCollection GeneralSection
        {
            get
            {
                if (!_iniData.Sections.ContainsSection(GeneralSectionName))
                {
                    _iniData.Sections.AddSection(GeneralSectionName);
                }

                return _iniData[GeneralSectionName];
            }
        }

        public SelectedWorkItemCheckinActionEnum SelectedWorkItemCheckinAction
        {
            get
            {
                var keyDataCollection = GeneralSection;
                if (!keyDataCollection.ContainsKey(SelectedWorkItemCheckinActionKeyName))
                {
                    keyDataCollection.AddKey(SelectedWorkItemCheckinActionKeyName);
                }

                var value = keyDataCollection[SelectedWorkItemCheckinActionKeyName];
                if (String.IsNullOrEmpty(value))
                    return SelectedWorkItemCheckinActionEnum.Resolve;

                return (SelectedWorkItemCheckinActionEnum)Enum.Parse(typeof(SelectedWorkItemCheckinActionEnum), value);
            }
            set
            {
                var keyDataCollection = GeneralSection;
                if (!keyDataCollection.ContainsKey(SelectedWorkItemCheckinActionKeyName))
                {
                    keyDataCollection.AddKey(SelectedWorkItemCheckinActionKeyName);
                }

                keyDataCollection[SelectedWorkItemCheckinActionKeyName] = value.ToString();
            }
        }

        public string PreviouslySelectedWorkItemQueriesValueSeperator
        {
            get
            {
                var keyDataCollection = GeneralSection;
                if (!keyDataCollection.ContainsKey(PreviouslySelectedWorkItemQueriesValueSeperatorKeyName))
                {
                    keyDataCollection.AddKey(PreviouslySelectedWorkItemQueriesValueSeperatorKeyName, "||");
                }

                var value = keyDataCollection[PreviouslySelectedWorkItemQueriesValueSeperatorKeyName];

                return value;
            }
            set
            {
                var keyDataCollection = GeneralSection;
                if (!keyDataCollection.ContainsKey(PreviouslySelectedWorkItemQueriesValueSeperatorKeyName))
                {
                    keyDataCollection.AddKey(PreviouslySelectedWorkItemQueriesValueSeperatorKeyName, "||");
                }

                keyDataCollection[PreviouslySelectedWorkItemQueriesValueSeperatorKeyName] = value;
            }
        }

        public int PreviouslySelectedWorkItemQueriesMaxCount
        {
            get
            {
                var keyDataCollection = GeneralSection;
                if (!keyDataCollection.ContainsKey(PreviouslySelectedWorkItemQueriesMaxCountKeyName))
                {
                    keyDataCollection.AddKey(PreviouslySelectedWorkItemQueriesMaxCountKeyName, 5.ToString());
                }

                return Convert.ToInt32(keyDataCollection[PreviouslySelectedWorkItemQueriesMaxCountKeyName]);
            }
            set
            {
                var keyDataCollection = GeneralSection;
                if (!keyDataCollection.ContainsKey(PreviouslySelectedWorkItemQueriesMaxCountKeyName))
                {
                    keyDataCollection.AddKey(PreviouslySelectedWorkItemQueriesMaxCountKeyName, 5.ToString());
                }

                keyDataCollection[PreviouslySelectedWorkItemQueriesMaxCountKeyName] = value.ToString();
            }
        }
    
        #endregion

        #region Shelving

        private KeyDataCollection ShelvingSection
        {
            get
            {
                if (!_iniData.Sections.ContainsSection(ShelvingSectionName))
                {
                    _iniData.Sections.AddSection(ShelvingSectionName);
                }

                return _iniData[ShelvingSectionName];
            }
        }

        public bool PreserveShelvedChangesLocally
        {
            get
            {
                var keyDataCollection = ShelvingSection;
                if (!keyDataCollection.ContainsKey(PreserveShelvedChangesLocallyKeyName))
                {
                    keyDataCollection.AddKey(PreserveShelvedChangesLocallyKeyName);
                }

                var value = keyDataCollection[PreserveShelvedChangesLocallyKeyName];
                if (String.IsNullOrEmpty(value))
                    return false;

                return bool.Parse(value);
            }
            set
            {
                var keyDataCollection = ShelvingSection;
                if (!keyDataCollection.ContainsKey(PreserveShelvedChangesLocallyKeyName))
                {
                    keyDataCollection.AddKey(PreserveShelvedChangesLocallyKeyName);
                }

                keyDataCollection[PreserveShelvedChangesLocallyKeyName] = value.ToString();
            }
        }

        public bool EvaluatePoliciesDuringShelve
        {
            get
            {
                var keyDataCollection = ShelvingSection;
                if (!keyDataCollection.ContainsKey(EvaluatePoliciesDuringShelveKeyName))
                {
                    keyDataCollection.AddKey(EvaluatePoliciesDuringShelveKeyName);
                }

                var value = keyDataCollection[EvaluatePoliciesDuringShelveKeyName];
                if (String.IsNullOrEmpty(value))
                    return false;

                return bool.Parse(value);
            }
            set
            {
                var keyDataCollection = ShelvingSection;
                if (!keyDataCollection.ContainsKey(EvaluatePoliciesDuringShelveKeyName))
                {
                    keyDataCollection.AddKey(EvaluatePoliciesDuringShelveKeyName);
                }

                keyDataCollection[EvaluatePoliciesDuringShelveKeyName] = value.ToString();
            }
        }

        #endregion
     
        #region PreviouslySelectedWorkItemsQueries

        private KeyDataCollection PreviouslySelectedWorkItemsQueriesSection
        {
            get
            {
                if (!_iniData.Sections.ContainsSection(PreviouslySelectedWorkItemQueriesSectionName))
                {
                    _iniData.Sections.AddSection(PreviouslySelectedWorkItemQueriesSectionName);
                }

                return _iniData[PreviouslySelectedWorkItemQueriesSectionName];
            }
        }

        private string[] GetPreviouslySelectedWorkItemsQueries(string teamProjectCollectionUri)
        {
            var keyDataCollection = PreviouslySelectedWorkItemsQueriesSection;
            if (!keyDataCollection.ContainsKey(teamProjectCollectionUri))
            {
                keyDataCollection.AddKey(teamProjectCollectionUri);
            }

            return keyDataCollection[teamProjectCollectionUri].Split(new[] { PreviouslySelectedWorkItemQueriesValueSeperator }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void SetPreviouslySelectedWorkItemsQueries(string teamProjectCollectionUri, string[] value)
        {
            var keyDataCollection = PreviouslySelectedWorkItemsQueriesSection;
            if (!keyDataCollection.ContainsKey(teamProjectCollectionUri))
            {
                keyDataCollection.AddKey(teamProjectCollectionUri);
            }

            keyDataCollection[teamProjectCollectionUri] = string.Join(PreviouslySelectedWorkItemQueriesValueSeperator, value);
        }

        public class PreviouslySelectedWorkItemsQueriesCollection
        {
            private readonly TeamPilgrimSettings _teamPilgrimSettings;

            internal PreviouslySelectedWorkItemsQueriesCollection(TeamPilgrimSettings teamPilgrimSettings)
            {
                _teamPilgrimSettings = teamPilgrimSettings;
            }

            public string[] this[string teamProjectCollectionUri]
            {
                get { return _teamPilgrimSettings.GetPreviouslySelectedWorkItemsQueries(teamProjectCollectionUri); }
                set { _teamPilgrimSettings.SetPreviouslySelectedWorkItemsQueries(teamProjectCollectionUri, value); }
            }
        }

        public PreviouslySelectedWorkItemsQueriesCollection PreviouslySelectedWorkItemsQueries { get; private set; }

        public void RemovePreviouslySelectedWorkItemQuery(string teamProjectCollectionUri, string workItemQueryPath)
        {
            var strings = TeamPilgrimPackage.TeamPilgrimSettings.PreviouslySelectedWorkItemsQueries[teamProjectCollectionUri];

            var list = new List<string>(strings);
            list.Remove(workItemQueryPath);

            TeamPilgrimPackage.TeamPilgrimSettings.PreviouslySelectedWorkItemsQueries[teamProjectCollectionUri] =
                list.Distinct().Take(TeamPilgrimPackage.TeamPilgrimSettings.PreviouslySelectedWorkItemQueriesMaxCount).ToArray();

            TeamPilgrimPackage.TeamPilgrimSettings.Save();
        }

        public void AddPreviouslySelectedWorkItemQuery(string teamProjectCollectionUri, string workItemQueryPath)
        {
            var strings = TeamPilgrimPackage.TeamPilgrimSettings.PreviouslySelectedWorkItemsQueries[teamProjectCollectionUri];

            var list = new List<string>(strings);
            list.Insert(0, workItemQueryPath);

            TeamPilgrimPackage.TeamPilgrimSettings.PreviouslySelectedWorkItemsQueries[teamProjectCollectionUri]=
                list.Distinct().Take(TeamPilgrimPackage.TeamPilgrimSettings.PreviouslySelectedWorkItemQueriesMaxCount).ToArray();

            TeamPilgrimPackage.TeamPilgrimSettings.Save();
        }

        #endregion

        public void Save()
        {
            _parser.SaveFile(GetSettingsFilePath, _iniData);
        }
    }
}