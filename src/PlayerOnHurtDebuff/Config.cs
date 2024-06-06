using Newtonsoft.Json;
using Terraria.ID;

namespace PlayerOnHurtDebuff
{
    public class Pair<T, U>
    {
        public Pair(T min, U max)
        {
            Min = min;
            Max = max;
        }

        public T Min { get; set; }
        public U Max { get; set; }
    }
    public class BuffProperties
    {
        public int InflictBuffID;
        public Pair<int, int> Duration;
        public float Chance;
    }
    public class Config
    {
        [JsonProperty]
        public Dictionary<int, List<BuffProperties>> Projectiles { get; set; }

        [JsonProperty]
        public Dictionary<int, List<BuffProperties>> Npcs { get; set; }

        public static void Write(string path)
        {
            Config config = new()
            {
                Projectiles = new Dictionary<int, List<BuffProperties>>
                {
                    {
                        ProjectileID.JavelinHostile,
                        new List<BuffProperties>
                        {
                            new BuffProperties
                            {
                                InflictBuffID = BuffID.Weak,
                                Duration = new Pair<int, int>(20, 40),
                                Chance = 0.5f
                            },
                            new BuffProperties
                            {
                                InflictBuffID = BuffID.Bleeding,
                                Duration = new Pair<int, int>(10, 30),
                                Chance = 1f
                            }
                        }
                    },
                    {
                        ProjectileID.FlamingArrow,
                        new List<BuffProperties>()
                        {
                            new BuffProperties
                            {
                                InflictBuffID = BuffID.WitheredArmor,
                                Duration = new Pair<int, int>(20, 40),
                                Chance = 0.25f
                            }
                        }
                    }
                },

                Npcs = new Dictionary<int, List<BuffProperties>>
                {
                    {
                        NPCID.BoneLee,
                        new List<BuffProperties>()
                        {
                            new BuffProperties
                            {
                                InflictBuffID = BuffID.Darkness,
                                Duration = new Pair<int, int>(5, 10),
                                Chance = 0.75f
                            },
                            new BuffProperties
                            {
                                InflictBuffID = BuffID.Slow,
                                Duration = new Pair<int, int>(10, 20),
                                Chance = 0.1f
                            }
                        }
                    }
                }
            };

            using (StreamWriter sw = new(path))
            {
                sw.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
            }
        }
        public static Config Read(string path)
        {
            using (StreamReader sr = new(path))
            {
                return JsonConvert.DeserializeObject<Config>(sr.ReadToEnd()) ?? new Config();
            }
        }
    }
}
