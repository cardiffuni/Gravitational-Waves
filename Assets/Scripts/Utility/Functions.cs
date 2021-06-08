using Game.Core;
using Game.Extensions;
using Game.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Utility {
    public static class Functions {

        public static readonly double MetersFeetRatio = 0.304800610f;

        private readonly static List<string> FirstNamesList = new List<string>() { "Oliver", "George", "Harry", "Noah", "Jack", "Charlie", "Leo", "Jacob", "Freddie", "Alfie", "Archie", "Theo", "Oscar", "Arthur", "Thomas", "Logan", "Henry", "Joshua", "James", "William", "Max", "Isaac", "Lucas", "Ethan", "Teddy", "Finley", "Mason", "Harrison", "Hunter", "Alexander", "Daniel", "Joseph", "Tommy", "Arlo", "Reggie", "Edward", "Jaxon", "Adam", "Sebastian", "Rory", "Riley", "Dylan", "Elijah", "Carter", "Albie", "Louie", "Toby", "Benjamin", "Reuben", "Jude", "Samuel", "Harley", "Luca", "Frankie", "Ronnie", "Jenson", "Hugo", "Jake", "David", "Theodore", "Roman", "Bobby", "Alex", "Caleb", "Ezra", "Ollie", "Finn", "Jackson", "Zachary", "Jayden", "Harvey", "Albert", "Lewis", "Blake", "Stanley", "Elliot", "Grayson", "Liam", "Louis", "Matthew", "Elliott", "Tyler", "Luke", "Michael", "Gabriel", "Ryan", "Dexter", "Kai", "Jesse", "Leon", "Nathan", "Ellis", "Connor", "Jamie", "Rowan", "Sonny", "Dominic", "Eli", "Aaron", "Jasper", "Olivia", "Amelia", "Isla", "Ava", "Emily", "Sophia", "Grace", "Mia", "Poppy", "Ella", "Lily", "Evie", "Isabella", "Sophie", "Ivy", "Freya", "Harper", "Willow", "Charlotte", "Jessica", "Rosie", "Daisy", "Alice", "Elsie", "Sienna", "Florence", "Evelyn", "Phoebe", "Aria", "Ruby", "Isabelle", "Esme", "Scarlett", "Matilda", "Sofia", "Millie", "Eva", "Layla", "Chloe", "Luna", "Maisie", "Lucy", "Erin", "Eliza", "Ellie", "Mila", "Imogen", "Bella", "Lola", "Molly", "Maya", "Violet", "Lilly", "Holly", "Thea", "Emilia", "Hannah", "Penelope", "Harriet", "Georgia", "Emma", "Lottie", "Nancy", "Rose", "Amber", "Elizabeth", "Gracie", "Zara", "Darcie", "Summer", "Hallie", "Aurora", "Ada", "Anna", "Orla", "Robyn", "Bonnie", "Abigail", "Darcy", "Eleanor", "Arabella", "Lexi", "Clara", "Heidi", "Lyla", "Annabelle", "Jasmine", "Nevaeh", "Victoria", "Amelie", "Myla", "Maria", "Julia", "Niamh", "Mya", "Annie", "Darcey", "Zoe", "Felicity", "Iris", "Callum", "Ben", "Owen", "Patrick", "Rhys", "Marcus", "Robert", "Charles", "Danny", "Frederick", "John", "Anthony", "Joe", "Christian", "Taylor", "Zack", "Kyle", "Ewan", "Miles", "Kevin", "Nathaniel", "Zak", "Tobias", "Christopher", "Bradley", "Cole", "Tom", "Jason", "Declan", "Jonathan", "Andrew", "Sean", "Morgan", "Maxwell", "Joel", "Aiden", "Nicholas", "Alfred", "Josh", "Tristan", "Peter", "Brandon", "Mark", "Scott", "Euan", "Mitchell" };
        private readonly static List<string> LastNamesList = new List<string>() { "Miller", "Johnson", "Jones", "Smith", "Davis", "Williams", "Brown", "Anderson", "Moore", "Thomas", "Martin", "Jackson", "Wilson", "Taylor", "Wright", "Roberts", "Adams", "White", "Robinson", "Harris", "Baker", "Green", "Mitchell", "Hall", "Phillips", "Clark", "Hill", "Thompson", "Walker", "Carter", "Young", "Allen", "Campbell", "Scott", "Lewis", "King", "Nelson", "Hayes", "Price", "Stephens", "Parker", "Morris", "Sanders", "Grant", "Cook", "Daniels", "Ward", "Stewart", "Watson", "Bryant", "Richardson", "Webb", "Stevens", "Henderson", "Rogers", "Hamilton", "Fisher", "Wood", "Owens", "Coleman", "Butler", "Howard", "Gibson", "Hughes", "Shaw", "Bell", "Woods", "Burns", "Rose", "Cox", "Spencer", "Hicks", "Dixon", "Ryan", "Foster", "Brooks", "Payne", "Myers", "West", "Gordon", "Wells", "Holmes", "Griffin", "Stone", "Edwards", "Turner", "Crawford", "Ellis", "Mason", "Simmons", "Russell", "Jordan", "Morgan", "Robertson", "Palmer", "Cooper", "Gardner", "Cole", "Simpson", "Nichols" };

        private static Stack<string> FirstNames;
        private static Stack<string> LastNames;

        public static double MetersToFeet(double meters) {
            return meters / MetersFeetRatio;
        }

        public static double FeetToMeters(double feet) {
            return feet * MetersFeetRatio;
        }

        public static string MetersToDisplay(double meters) {
            return string.Format("{0:0.0}m ({1:0}ft)", Math.Round(meters, 1, SettingsManager.MidpointRounding), Math.Round(MetersToFeet(meters), SettingsManager.MidpointRounding));
        }

        static Functions() {
            FirstNamesList.Shuffle();
            FirstNames = new Stack<string>(FirstNamesList);
            LastNamesList.Shuffle();
            LastNames = new Stack<string>(LastNamesList);
        }


        public static Vector3 GenPos(int num, int idx, float radius, float height) {
            float angle = GenAngle(num, idx);
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            return new Vector3(x, height, z);
        }

        public static Quaternion GenRot(int num, int idx) {
            float angle = GenAngle(num, idx);
            float degrees = -angle * Mathf.Rad2Deg;
            return Quaternion.Euler(0, degrees, 0);
        }
        public static float GenAngle(int num, int idx) {
            return idx * Mathf.PI * 2 / num;
        }

        public static float Percentage(float percent, float num) {
            return percent / 100 * num;
        }
        public static int ValidateInt(string toCheck) {
            int value = 0;

            if (toCheck != null) {
                string stripped = Regex.Replace(toCheck, "[^\\-0-9]", "");
                if (stripped != "") {
                    value = int.Parse(stripped);
                }
            }

            return value;
        }

        public static int ValidatePositiveInt(int toCheck) {
            return toCheck >= 0 ? toCheck : 0;
        }

        public static int ValidatePositiveInt(string toCheck) {
            return ValidatePositiveInt(ValidateInt(toCheck));
        }


        public static string GenerateNewFirstName() {
            return FirstNames.Pop();
        }

        public static string GenerateNewLastName() {
            return LastNames.Pop();
        }

        internal static float Distance(ITarget target1, ITarget target2) {
            return Vector3.Distance(target1.transform.position, target2.transform.position);
        }

        public static int Round(float input, bool midPoint = SettingsManager.Midpoint, bool roundDown = SettingsManager.RoundDown, MidpointRounding midpointRounding = SettingsManager.MidpointRounding) {
            if (midPoint) {
                return (int)Math.Round(input, SettingsManager.MidpointRounding);
            } else if (roundDown) {
                return (int)Mathf.Floor(input);
            } else {
                return (int)Mathf.Ceil(input);
            }
        }

        public static T IndexerByID<T>(List<T> list, string id) where T : IIndexer {
            try {
                return list.First(s => s.ID.ToLower() == id.ToLower());
            } catch (InvalidOperationException e) {
                Debug.LogErrorFormat("Error: tried to get stat with ID: '{1}', error detail: {0}", e, id);
                return default;
            } catch {
                Debug.LogErrorFormat("Error: stat with ID: '{0}", id);
                return default;
            }
        }

        public static string SymbolOutputter(object s, int n = 1, string tab = "") {
            return string.Join(tab, Enumerable.Repeat(s, n));
        }

        public static void ExportToJson(object obj, string name = null, string folder = null, string path = SettingsManager.JsonDefaultPath) {

            string folderpath;
            if (name == null) {
                name = obj.ToString();
            }
            Debug.LogFormat("ExportToJson: {0}", name);
            string filename = string.Format("{0}.json", name);
            if (folder == null || folder == "") {
                folderpath = path;
            } else {
                folderpath = Path.Combine(path, folder);
                Directory.CreateDirectory(folderpath);
            }
            StreamWriter writer = new StreamWriter(Path.Combine(folderpath, filename), false);
            writer.Write(JsonConvert.SerializeObject(obj, SettingsManager.JsonSerializerFormatting, SettingsManager.JsonSerializerSettings));
            writer.Close();
        }

        public static string ObjectToStringJson(object obj) {
            //Debug.LogFormat("ReadJson: {0}", filename);
            return JsonConvert.SerializeObject(obj, SettingsManager.JsonSerializerFormatting, SettingsManager.JsonSerializerSettings);
        }

        public static T ReadJson<T>(TextAsset asset) {
            //Debug.LogFormat("ReadJson: {0}", filename);
            return JsonConvert.DeserializeObject<T>(asset.text, SettingsManager.JsonSerializerSettings);
        }


        public static T StringJsonToObject<T>(string rawjson) {
            //Debug.LogFormat("ReadJson: {0}", filename);
            return JsonConvert.DeserializeObject<T>(rawjson, SettingsManager.JsonSerializerSettings);
        }
    }
}