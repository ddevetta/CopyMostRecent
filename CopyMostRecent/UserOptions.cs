using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CopyMostRecent
{
    /// <summary>
    /// Containing Class for the User-alterable options
    /// </summary>
    /// <seealso cref="System.Configuration.ApplicationSettingsBase" />
    [SettingsProviderAttribute(typeof(UserOptionsSettingsProvider))]
    [XmlRoot(ElementName = "UserOptions")]
    public class UserOptions : ApplicationSettingsBase
    {
        /// <summary>
        /// Gets or sets the time window in milliseconds.
        /// This is used during the Modified date/time comparison between Source and Target.
        /// Times that are within this grace period are considered equal.
        /// This is to counteract slight time differences particularly if source and target are on different servers.
        /// The default is 2000 (2 seconds).
        /// </summary>
        /// <value>
        /// The time window in milliseconds.
        /// </value>
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("2000")]
        [XmlElement]
        public int TimeWindowMilliseconds
        {
            get { return (int)(this["TimeWindowMilliseconds"]); }
            set { this["TimeWindowMilliseconds"] = value; }
        }

        /// <summary>
        /// Gets or sets the ignore list.
        /// This is a newline-separated list of file ptterns to ignore.
        /// It uses regex pattern matching rules.
        /// The default is one file - 'desktop.ini' which can be legitimately different in the source and destination directories.
        /// </summary>
        /// <value>
        /// The ignore list.
        /// </value>
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("desktop.ini")]
        [XmlElement]
        public string IgnoreList
        {
            get { return (string)(this["IgnoreList"]); }
            set { this["IgnoreList"] = value; }
        }

        /// <summary>
        /// Gets or sets the 'dummy copy' flag'.
        /// If this flag is set to true, then the copy operations are not actually performed but the copy
        /// function will delay for 250ms to simulate some copy time.
        /// This is not set in the user options dialog - set it manually in the file to alter it's value.
        /// </summary>
        /// <value>
        /// true or false.
        /// </value>
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("false")]
        [XmlElement]
        public bool DummyCopy
        {
            get { return (bool)(this["DummyCopy"]); }
            set { this["DummyCopy"] = value; }
        }
    }

    /// <summary>
    /// The settings provider for the UserOptions settings class, overriding the default provider.
    /// This was developed so that AppData was not versioned, so the user didn't lose the saved options
    /// when upgrading.
    /// </summary>
    /// <seealso cref="System.Configuration.SettingsProvider" />
    sealed class UserOptionsSettingsProvider : SettingsProvider
    {
        public new string Name = Assembly.GetExecutingAssembly().GetName().Name;

        public override string ApplicationName { get { return Name; } set { } }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            UserOptionsValueCollection options = new UserOptionsValueCollection(this.ApplicationName);
            options.Load(context, properties);
            return options;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            UserOptionsValueCollection options = new UserOptionsValueCollection(this.ApplicationName);
            options.Save(context, values);
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            name = name ?? Name;
            config = config ?? new NameValueCollection();
            base.Initialize(name, config);
        }
    }

    /// <summary>
    /// This Loads or Saves the UserOptions to the 'user.config' file in 'CopyIfNewer folder of the roaming AppData location.
    /// The resultant file is similar in look and shape to the default config XML structure created by the default provider,
    /// but there's no guarantee it can be read by the default provider.
    /// </summary>
    /// <seealso cref="System.Configuration.SettingsPropertyValueCollection" />
    sealed class UserOptionsValueCollection : SettingsPropertyValueCollection
    {
        private string file;

        public UserOptionsValueCollection(string applicationName)
        { 
            file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName, "user.config");
        }

        public void Load(SettingsContext context, SettingsPropertyCollection properties)
        { 
            NameValueCollection userOptions = new NameValueCollection();
            XmlDocument xml = new XmlDocument();
            if (File.Exists(file))  
            {
                xml.Load(file);
                if (xml.HasChildNodes)
                {
                    XmlNodeList nodes = xml.GetElementsByTagName("setting");
                    foreach (XmlNode node in nodes)
                    {
                        userOptions.Add(((XmlElement)node).GetAttribute("name"), node.InnerText.Trim());
                    }
                }
            }
            foreach (SettingsProperty property in properties)
            {
                SettingsPropertyValue prop = new SettingsPropertyValue(property);
                string[] values = userOptions.GetValues(prop.Name);
                if (values != null)     // If the prop exists in the UserOptions file, override the default value
                {
                    prop.SerializedValue = values[0];
                }
                this.Add(prop);
            }
        }

        public void Save(SettingsContext context, SettingsPropertyValueCollection values)
        {
            XmlDocument xml = new XmlDocument();
            XmlElement conf = xml.CreateElement("configuration");
            XmlElement user = xml.CreateElement("userSettings");
            XmlElement outer = xml.CreateElement(context["GroupName"].ToString());
            foreach (SettingsPropertyValue value in values)
            {
                XmlElement prop = xml.CreateElement("setting");
                prop.SetAttribute("name", value.Name);
                prop.SetAttribute("serializeAs", "String");
                XmlElement val = xml.CreateElement("value");
                val.InnerText = value.PropertyValue.ToString().Trim();
                prop.AppendChild(val);
                outer.AppendChild(prop);
            }
            user.AppendChild(outer);
            conf.AppendChild(user);
            xml.AppendChild(conf);
            xml.Save(file);
        }
    }
}
