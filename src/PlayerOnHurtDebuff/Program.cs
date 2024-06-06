using Terraria;
using TerrariaApi.Server;
using Terraria.DataStructures;
using TShockAPI;
using TShockAPI.Hooks;
using Terraria.ID;

namespace PlayerOnHurtDebuff
{
    [ApiVersion(2, 1)]
    public class PlayerOnHurtDebuff : TerrariaPlugin
    {
        public override string Author => "Sors";

        public override string Description => "Make npcs and projectiles inflict debuff on hit.";

        public override string Name => "PlayerOnHurtDebuff";

        public override Version Version => new(1, 0, 0);

        public Dictionary<int, List<BuffProperties>> ModProjs { get; set; } = new Dictionary<int, List<BuffProperties>>();
        public Dictionary<int, List<BuffProperties>> ModNpcs { get; set; } = new Dictionary<int, List<BuffProperties>>();

        public const string ConfigPath = "PlayerOnHurtDebuff.json";

        public PlayerOnHurtDebuff(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += OnReload;
            ServerApi.Hooks.NetGetData.Register(this, OnNetGetData);

            string path = Path.GetFullPath(Path.Combine(TShock.SavePath, ConfigPath));
            if (!File.Exists(path))
                Config.Write(path);
            Config settings = Config.Read(path);

            foreach (KeyValuePair<int, List<BuffProperties>> projectile in settings.Projectiles)
                ModProjs.Add(projectile.Key, projectile.Value);
            foreach (KeyValuePair<int, List<BuffProperties>> npc in settings.Npcs)
                ModNpcs.Add(npc.Key, npc.Value);
        }
    
        public static int ConvertDurationBasedOnWorldMode(int duration)
        {
            if (Main.expertMode)
                return (int)(duration / Main.GameModeInfo.DebuffTimeMultiplier);
            if (Main.masterMode)
                return (int)(duration / Main.GameModeInfo.DebuffTimeMultiplier);
            return duration;
        }

        private void OnNetGetData(GetDataEventArgs args)
        {
            PacketTypes msgID = args.MsgID;
            if (msgID == PacketTypes.PlayerHurtV2)
            {
                using (BinaryReader br = new(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                {
                    byte playerID = br.ReadByte();
                    PlayerDeathReason deathReason = PlayerDeathReason.FromReader(br);
                    short damage = br.ReadInt16();
                    byte hitDir = br.ReadByte();
                    BitsByte flags = br.ReadByte();
                    sbyte cd = br.ReadSByte();
                    
                    Player player = Main.player[playerID];
                    NPC npc = (deathReason._sourceNPCIndex == -1) ? new() : Main.npc[deathReason._sourceNPCIndex];
                    if (player.dead || !player.active || player.ghost)
                        return;
                    if (ModProjs.ContainsKey(deathReason._sourceProjectileType))
                    {
                        foreach (BuffProperties buff in ModProjs[deathReason._sourceProjectileType])
                        {
                            if (Main.projectile[deathReason._sourceProjectileLocalIndex].owner == 255 && Main.rand.NextFloat() <= buff.Chance)
                            {
                                byte[] addBuff = new PacketFactory()
                                    .SetType((short)PacketTypes.PlayerAddBuff)
                                    .PackByte(playerID)
                                    .PackUInt16((ushort)buff.InflictBuffID)
                                    .PackInt32(ConvertDurationBasedOnWorldMode(Main.rand.Next(buff.Duration.Min * 60, buff.Duration.Max * 60)))
                                    .GetByteData();

                                TShock.Players[playerID].SendRawData(addBuff);
                            }
                        }
                    }
                    if (ModNpcs.ContainsKey(npc.type))
                    {
                        foreach (BuffProperties buff in ModNpcs[npc.type])
                        {
                            if (Main.rand.NextFloat() <= buff.Chance)
                            {
                                byte[] addBuff = new PacketFactory()
                                    .SetType((short)PacketTypes.PlayerAddBuff)
                                    .PackByte(playerID)
                                    .PackUInt16((ushort)buff.InflictBuffID)
                                    .PackInt32(ConvertDurationBasedOnWorldMode(Main.rand.Next(buff.Duration.Min * 60, buff.Duration.Max * 60)))
                                    .GetByteData();

                                TShock.Players[playerID].SendRawData(addBuff);
                            }
                        }
                    }
                }
            }
        }

        private void OnReload(ReloadEventArgs args)
        {
            ModProjs.Clear(); ModNpcs.Clear();

            string path = Path.Combine(TShock.SavePath, ConfigPath);
            if (!File.Exists(path))
                Config.Write(path);
            Config settings = Config.Read(path);
 
            foreach (KeyValuePair<int, List<BuffProperties>> projectile in settings.Projectiles)
                ModProjs.Add(projectile.Key, projectile.Value);
            foreach (KeyValuePair<int, List<BuffProperties>> npc in settings.Npcs)
                ModNpcs.Add(npc.Key, npc.Value);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ModProjs.Clear(); ModNpcs.Clear();
                ServerApi.Hooks.NetGetData.Deregister(this, OnNetGetData);
            }
            base.Dispose(disposing);
        }
    }
}
