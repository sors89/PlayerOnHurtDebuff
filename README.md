# PlayerOnHurtDebuff
This plugin allows npcs and projectiles to inflict debuff with custom duration and chance to player.

# How to install
Grab a __PlayerOnHurtDebuff.dll__ at [here](https://github.com/sors89/PlayerOnHurtDebuff/releases/tag/v1.0.0) and drop it in `ServerPlugins` folder.

# Configuration
If there is no config file, the plugin will automatically create a `PlayerOnHurtDebuff.json`  in `tshock` directory. </br>
Example config file:
```
{
  "Projectiles": {
    "508": [
      {
        "InflictBuffID": 33,
        "Duration": {
          "Min": 20,
          "Max": 40
        },
        "Chance": 0.5
      }
    ]
  },
  "Npcs": {
    "287": [
      {
        "InflictBuffID": 22,
        "Duration": {
          "Min": 5,
          "Max": 10
        },
        "Chance": 0.75
      }
    ]
  }
}
```
**508** is JavelinHostile's Projectile ID and **287** is BoneLee's NPC ID. Both should be hostile.  FYI [Projectile ID](https://terraria.wiki.gg/wiki/Projectile_IDs) and [NPC ID](https://terraria.wiki.gg/wiki/NPC_IDs)</br>
**InflictBuffID**: Buff ID that you want to inflict when it hit player. (e.g 195 is Withered Armor debuff). </br>
**Duration**: must be in seconds. `Min` is minimum buff duration and `Max` is maximum buff duration. The plugin will choose randomly in (Min, Max) range. </br> 
**Chance**: chance to actually inflict buff to player. 0 < `chance` <= 1. Remember that 1 equals to 100%, 0.75 equals to 75%, 0.5 equals to 50% and so on... </br>

**Note 1**: You can add multiple buffs per projectile or npc. </br>
**Note 2**: Friendly projectile will not inflict debuff. </br>
Here is an example of adding multiple buffs to projectile and npc: </br>
```
{
  "Projectiles": {
    "508": [
      {
        "InflictBuffID": 33,
        "Duration": {
          "Min": 20,
          "Max": 40
        },
        "Chance": 0.5
      },
      {
        "InflictBuffID": 30,
        "Duration": {
          "Min": 10,
          "Max": 30
        },
        "Chance": 1.0
      }
    ],
    "82": [
      {
        "InflictBuffID": 195,
        "Duration": {
          "Min": 20,
          "Max": 40
        },
        "Chance": 0.25
      }
    ]
  },
  "Npcs": {
    "287": [
      {
        "InflictBuffID": 22,
        "Duration": {
          "Min": 5,
          "Max": 10
        },
        "Chance": 0.75
      },
      {
        "InflictBuffID": 32,
        "Duration": {
          "Min": 10,
          "Max": 20
        },
        "Chance": 0.1
      }
    ]
  }
}
```
