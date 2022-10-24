using System.Reflection;
using System.Text;
namespace CarsAndTanks.Settings;

/// <summary>
/// Provides save/load configuration for each adjustable settings.
/// Settings to persist must be setter/getters and "public" as that's what it enumerates.
/// </summary>
internal class Config
{
    /// <summary>
    /// Configuration filename template. {{prefix}} is replaced.
    /// </summary>
    const string c_configTemplateFilename = @"{{prefix}}-ait.cfg";

    internal bool ConfigChangedTheCarsAndNeuralNetworkAreInvalid { get; set; }

    /// <summary>
    /// Singleton instance of the config.
    /// </summary>
    internal static Config s_settings;

    /// <summary>
    /// Configuration of the world.
    /// </summary>
    internal ConfigWorld World = new();

    /// <summary>
    /// Configuration of the LIDAR (visual detection).
    /// </summary>
    internal ConfigAI AI = new();

    /// <summary>
    /// Configuration of the display.
    /// </summary>
    internal ConfigDisplay Display = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <exception cref="Exception"></exception>
    internal Config()
    {
        // stop creation of multiple instances of the class
        if (s_settings != null) throw new Exception("Singleton");

        s_settings = this;

        Load();

        AI.Layers[0] = AI.SamplePoints + AI.OtherSensorCount;
        AI.Layers[^1] = s_settings.AI.CountOfOutputNeuronsRequiredBasedOnModulation;
    }

    /// <summary>
    /// Saves the "settings" to a file.
    /// </summary>
    /// <param name="filename"></param>
    internal static void Save(string filename = c_configTemplateFilename)
    {
        filename = Path.Combine(Program.applicationUserSettings, filename);

        Save(s_settings.Display, filename, "display");
        Save(s_settings.World, filename, "world");
        Save(s_settings.AI, filename, "ai");
    }

    /// <summary>
    /// Loads the "settings" from a file.
    /// </summary>
    /// <param name="filename"></param>
    internal static void Load(string filename = c_configTemplateFilename)
    {
        filename = Path.Combine(Program.applicationUserSettings, filename);

        Load(s_settings.Display, filename, "display");
        Load(s_settings.World, filename, "world");
        Load(s_settings.AI, filename, "ai");
    }

    /// <summary>
    /// Used to avoid saving occurring whilst we change things.
    /// </summary>
    internal static bool s_blockConfigSavingWhilstWeUpdateThings = false;

    /// <summary>
    /// Saves the config settings to a file.
    /// </summary>
    /// <param name="configObject"></param>
    /// <param name="filename"></param>
    /// <param name="prefix"></param>
    internal static void Save(object configObject, string filename, string prefix)
    {
        filename = filename.Replace("{{prefix}}", prefix);

        // if this is set, we must ignore otherwise we'll be resaving repeatedly for each field as we load.
        if (!s_blockConfigSavingWhilstWeUpdateThings)
        {
            StringBuilder sbConfig = new();

            Type type = configObject.GetType(); // Get type pointer to THIS class
            PropertyInfo[] properties = type.GetProperties();

            // loop over the public variables, if found in config file assign new value
            //    attrib1=value1
            //    attrib2=value2
            foreach (var p in properties)
            {
                object? temp = p.GetValue(configObject); // get the value of the attribute/field

                if (temp is int || temp is string || temp is Enum || temp is bool || temp is float)
                {
                    sbConfig.AppendLine(p.Name + "=" + temp.ToString());
                }
                else
                {
                    // arrays require special treatment so that when we deserialise we know the type
                    // [<type>,<value1>,<value2>,<value3>...>
                    if (temp is Array array)
                    {
                        string result = "";
                        string arraytype = "unknown";

                        foreach (var element in array)
                        {
                            if (element is float) arraytype = "float"; // this will be deserialised as float
                            if (element is int) arraytype = "int";
                            if (element is double) arraytype = "double";

                            result += element + ",";
                        }

                        result = result.Trim(',');

                        sbConfig.AppendLine($"{p.Name}=[{arraytype},{result}]");
                    }
                    if (temp is Point point)
                    {
                        sbConfig.AppendLine($"{p.Name}={point.X},{point.Y}");
                    }
                }
            }
            // write the config file as name-value pairs
            File.WriteAllText(filename, sbConfig.ToString());
        }
    }

    /// <summary>
    /// Loads the config file.
    /// </summary>
    internal static void Load(object configObject, string filename, string prefix)
    {
        filename = filename.Replace("{{prefix}}", prefix);

        // no config file, there is nothing to load...
        if (!File.Exists(filename))
        {
            return;
        }

        // as we load, we will be updating properties, if we don't set this, we'll be saving as we load!
        s_blockConfigSavingWhilstWeUpdateThings = true;

        try
        {
            // we get a list of attribute=value pairs, but don't blindly just assign it. It will evolve, and fields come and go.
            // therefore we only want to assign those that are current to avoid errors. 

            // read the config into a dictionary.
            Dictionary<string, string> ConfigValues = ParseConfigFilePairValuesIntoDictionary(filename);

            StringBuilder sb = new();

            Type type = configObject.GetType(); // Get type pointer
            PropertyInfo[] properties = type.GetProperties();

            // loop over the public variables, if found in config file assign new value
            foreach (var p in properties)
            {
                string name = p.Name;

                if (ConfigValues.ContainsKey(name)) // if this property has a corresponding value in the config, replace
                {
                    string valueFromConfig = ConfigValues[name];

                    p.SetValue(configObject, StringConfigValueToObjectType(p.GetValue(configObject), valueFromConfig));
                }
            }
        }
        finally
        {
            // this should always be "false" unless we are doing something such as the Load settings.
            s_blockConfigSavingWhilstWeUpdateThings = false;
        }
    }

    /// <summary>
    /// Reads the settings config file (stored as value-pairs), and parses it into a Dictionary.
    /// </summary>
    /// <param name="ConfigValues"></param>
    private static Dictionary<string, string> ParseConfigFilePairValuesIntoDictionary(string filename)
    {
        Dictionary<string, string> ConfigValues = new();

        // the present of the config file should always be checked BEFORE calling this.
        string[] valuePairs = File.ReadAllLines(filename);

        foreach (string valuePair in valuePairs)
        {
            // attrib1=value1
            //    0       1
            string[] tokens = valuePair.Split('=');

            if (tokens.Length == 2) // just in case!
            {
                ConfigValues.Add(tokens[0], tokens[1]);
            }
        }

        return ConfigValues; // dictionary of settings
    }

    /// <summary>
    /// Config isn't binary but a string representation of attrib=value pairs. 
    /// We need to cast the string value before assigning to a property using reflection.
    /// </summary>
    /// <param name="temp"></param>
    /// <param name="field"></param>
    /// <param name="valueFromConfig"></param>
    /// <returns></returns>
    private static object? StringConfigValueToObjectType(object? SourceProperty, string valueFromConfig)
    {
        if (SourceProperty is null) return null;

        if (SourceProperty is int)
        {
            return int.Parse(valueFromConfig);
        }
        else if (SourceProperty is string)
        {
            return valueFromConfig;
        }
        else if (SourceProperty is Enum obj)
        {
            return Enum.Parse(obj.GetType(), valueFromConfig);
        }
        else if (SourceProperty is bool)
        {
            return bool.Parse(valueFromConfig);
        }
        else if (SourceProperty is float)
        {
            return float.Parse(valueFromConfig);
        }
        else if (SourceProperty is double)
        {
            return double.Parse(valueFromConfig);
        }
        else if (SourceProperty is Array)
        {
            valueFromConfig = valueFromConfig[1..^1]; // remove []
            string[] valueArray = valueFromConfig.Split(',');
            string type = "";

            // we support arrays of float,int and double
            List<float> fvalues = new();
            List<int> ivalues = new();
            List<double> dvalues = new();

            foreach (string val in valueArray)
            {
                // first value is the "type" of the array
                if (string.IsNullOrWhiteSpace(type))
                {
                    type = val;
                    continue;
                }
                else
                {
                    // based on "type" we assign the relevant list with the value
                    if (type == "float") fvalues.Add(float.Parse(val));
                    if (type == "double") dvalues.Add(double.Parse(val));
                    if (type == "int") ivalues.Add(int.Parse(val));
                }
            }

            // based on the type, return the array we've populated.
            if (type == "float") return fvalues.ToArray();
            if (type == "double") return dvalues.ToArray();
            if (type == "int") return ivalues.ToArray();

            throw new Exception("unknown type");  // arrays of float, double or int only are supported
        }
        else if (SourceProperty is Point)
        {
            string[] valueArray = valueFromConfig.Split(',');
            return new Point(int.Parse(valueArray[0]), int.Parse(valueArray[1]));
        }
        else
        {
            // we don't know how to handle the "SourceProperty", better to throw than ignore otherwise
            // silent errors may arise
            throw new ArgumentException("Unrecognised SourceProperty");
        }
    }
}