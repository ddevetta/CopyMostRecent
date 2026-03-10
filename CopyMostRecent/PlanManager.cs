using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CopyMostRecent
{
    /// <summary>
    /// Class encapsulating all info for a single Plan.
    /// </summary>
    public class Plan
    {
        public string PlanName { get; set; } = string.Empty;
        public string Group { get; set; }
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public DateTime LastScanDate { get; set; }
        public DateTime LastRunDate { get; set; }

        public Plan() { }
        public Plan(string planName, string group, string sourceDirectory, string destinationDirectory) 
        { 
            this.PlanName = planName;
            this.Group = group;
            this.SourceDirectory = sourceDirectory;
            this.DestinationDirectory = destinationDirectory;
        }
    }

    /// <summary>
    /// Manager for the Plan List, containing Save, Load and search methods.
    /// The plan list is persised as a json text file in the file/folder supplied.
    /// </summary>
    public class PlanManager
    {
        /// <summary>
        /// The List of Plans.
        /// </summary>
        [JsonInclude]
        public List<Plan> Plans = new List<Plan>();

        /// <summary>
        /// Saves the specified Plans list.
        /// </summary>
        /// <param name="file">The folder and file to save as.</param>
        public async void Save(string file)
        {
            using (FileStream stream = File.Create(file))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                await JsonSerializer.SerializeAsync(stream, this, options);
            }
        }

        /// <summary>
        /// Loads the Plans list from the specified file.
        /// </summary>
        /// <param name="file">The file to load.</param>
        public void Load(string file)
        {
            if (!File.Exists(file))  // If no file exists then just clear this Plans list
            {
                Plans.Clear();
                return; 
            }

            using (FileStream stream = File.OpenRead(file))
            {
                PlanManager p = (PlanManager)JsonSerializer.Deserialize<PlanManager>(stream);
                this.Plans = p.Plans;
            }
        }

        /// <summary>
        /// Finds and returns the plan, by plan name.
        /// </summary>
        /// <param name="planName">Name of the plan.</param>
        /// <returns>The Plan if found, or null</returns>
        public Plan FindPlan(string planName)
        {
            return this.Plans.Find((p) => p.PlanName == planName);
        }

        /// <summary>
        /// Finds and returns the plan, by source/destination directory.
        /// </summary>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <returns>The Plan if found, or null</returns>
        public Plan FindPlan(string sourceDirectory, string destinationDirectory)
        {
            return this.Plans.Find((p) => p.SourceDirectory == sourceDirectory && p.DestinationDirectory == destinationDirectory);
        }

        /// <summary>
        /// Finds and returns all plans that have the supplied source directory.
        /// </summary>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <returns>A List of Plans, zero length if none.</returns>
        public List<Plan> FindPlans(string sourceDirectory)
        {
            return this.Plans.FindAll((p) => p.SourceDirectory == sourceDirectory);
        }

        /// <summary>
        /// Gets all the groups, sorted and deduplicated.
        /// </summary>
        /// <returns></returns>
        public string[] GetGroups()
        {
            List<string> result = new List<string>();
            foreach (Plan plan in Plans)
            { 
                if (plan.Group != null && plan.Group.Length > 0)
                    if (!result.Contains(plan.Group))
                        result.Add(plan.Group);
            }
            result.Sort();
            return result.ToArray();
        }

        public PlanManager() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanManager"/> class, and loads the Plans from the given file.
        /// </summary>
        /// <param name="loadFile">The load file.</param>
        public PlanManager(string loadFile) 
        { 
            this.Load(loadFile);
        }
    }

}
