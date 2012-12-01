using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using IniParser;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;

namespace JustAProgrammer.TeamPilgrim.VisualStudio
{
    public class TeamPilgrimSettings
    {
        private const string GeneralSectionName = "General";
        private const string SelectedWorkItemCheckinActionKeyName = "SelectedWorkItemCheckinAction";

        static TeamPilgrimSettings()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.Assert(directoryName != null, "directoryName != null");

            GetSettingsFilePath = Path.Combine(directoryName, @"teampilgrim.ini");
        }

        private static readonly string GetSettingsFilePath;

        private readonly IniData _iniData;
        private readonly FileIniDataParser _parser;

        private KeyDataCollection GeneralSection
        {
            get
            {
                if (_iniData[GeneralSectionName] == null)
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
                Save();
            }
        }

        public void Save()
        {
            _parser.SaveFile(GetSettingsFilePath, _iniData);
        }
    }
}