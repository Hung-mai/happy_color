using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enums
{
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static List<T> SortOrder<T>(List<T> list)
    {
        return list.OrderBy(d => System.Guid.NewGuid()).Take(list.Count).ToList();
    }
}

public enum GameMode { Normal, Paint, SortBonus}

public enum GameState { BlockAction , MainMenu, Pause, GamePlay, Win, Lose}

public enum BrickType { NormalCube, LegoCube, ConNhong, LegoLoi, LegoLom }
public enum HammerType { NormalHammer, Hammer1, Hammer2, Hammer3, Hammer4, Hammer5, Firework, Bomb}
public enum ColorType
{
    Editor = 0,
    Clear = 1,      //(255,255,255)
    White,          //(254,254,254)
    Black,          //(0,0,0)

    RedBerry_1,     //(152,0,0)
    RedBerry_2,     //(230,184,175)
    RedBerry_3,     //(221,126,107)
    RedBerry_4,     //(204,65,37)
    RedBerry_5,     //(166,28,0)
    RedBerry_6,     //(133,32,12)

    Red_1,          //(255,0,0)
    Red_2,          //(244,204,204)
    Red_3,          //(234,153,153)
    Red_4,          //(224,102,102)
    Red_5,          //(204,0,0)
    Red_6,          //(153,0,0)

    Orange_1,       //(255,153,0)
    Orange_2,       //(252,229,205)
    Orange_3,       //(249,203,156)
    Orange_4,       //(246,178,107)
    Orange_5,       //(230,145,56)
    Orange_6,       //(180,95,6)

    Yellow_1,       //(255,255,0)
    Yellow_2,       //(255,242,204)
    Yellow_3,       //(255,229,153)
    Yellow_4,       //(255,217,102)
    Yellow_5,       //(241,194,50)
    Yellow_6,       //(191,144,0)

    Green_1,        //(0,255,0)
    Green_2,        //(217,234,211)
    Green_3,        //(182,215,168)
    Green_4,        //(147,196,125)
    Green_5,        //(106,168,79)
    Green_6,        //(56,118,29)

    Cyan_1,         //(0,255,255)
    Cyan_2,         //(208,224,227)
    Cyan_3,         //(162,196,201)
    Cyan_4,         //(118,165,175)
    Cyan_5,         //(69,129,142)
    Cyan_6,         //(19,79,92)

    Blue_1,         //(74,134,232)
    Blue_2,         //(208,224,227)
    Blue_3,         //(164,194,244)
    Blue_4,         //(109,158,235)
    Blue_5,         //(60,120,216)
    Blue_6,         //(17,85,204)

    SeaBlue_1,      //(0,0,255)
    SeaBlue_2,      //(207,226,243)
    SeaBlue_3,      //(164,194,244)
    SeaBlue_4,      //(109,158,235)
    SeaBlue_5,      //(61,133,198)
    SeaBlue_6,      //(11,83,148)

    Purple_1,       //(153,0,255)
    Purple_2,       //(217,210,233)
    Purple_3,       //(180,167,214)
    Purple_4,       //(142,124,195)
    Purple_5,       //(103,78,167)
    Purple_6,       //(53,28,117)

    Magenta_1,      //(255,0,255)
    Magenta_2,      //(234,209,220)
    Magenta_3,      //(213,166,189)
    Magenta_4,      //(194,123,160)
    Magenta_5,      //(166,77,121)
    Magenta_6,      //(116,27,71)

    Grey_1,         //(243,243,243)
    Grey_2,         //(215,215,215)
    Grey_3,         //(200,200,200)
    Grey_4,         //(160,160,160)
    Grey_5,         //(120,120,120)
    Grey_6,         //(100,100,100)
    Grey_7,         //(70,70,70)
    Grey_8,         //(50,50,50)
}

public enum ItemType
{
    id1,
    id2,
    id3,
    id4,
    id5,
    id6,
    id7,
    id8,
    id9,
    id10,
    id11,
    id12,
    id13,
    id14,
    id15,
    id16,
    id17,
    id18,
    id19,
    id20,
    id21,
    id22,
    id23,
    id24,
    id25,
    id26,
    id27,
    id28,
    id29,
    id30,
    id31,
    id32,
    id33,
    id34,
    id35,
    id36,
    id37,
    id38,
    id39,
    id40,
    id41,
    id42,
    id43,
    id44,
    id45,
    id46,
    id47,
    id48,
    id49,
    id50,
    id51,
    id52,
    id53,
    id54,
    id55,
    id56,
    id57,
    id58,
    id59,
    id60,
    id61,
    id62,
    id63,
    id64,
    id65,
    id66,
    id67,
    id68,
    id69,
    id70,
    id71,
    id72,
    id73,
    id74,
    id75,
    id76,
    id77,
    id78,
    id79,
    id80,
    id81,
    id82,
    id83,
    id84,
    id85,
    id86,
    id87,
    id88,
    id89,
    id90,
    id91,
    id92,
    id93,
    id94,
    id95,
    id96,
    id97,
    id98,
    id99,
    id100,
    id101,
    id102,
    id103,
    id104,
    id105,
    id106,
    id107,
    id108,
    id109,
    id110,
    id111,
    id112,
    id113,
    id114,
    id115,
    id116,
    id117,
    id118,
    id119, id120, id121, id122, id123, id124, id125, id126, id127, id128, id129, id130, id131, id132, id133, id134, id135, id136, id137, id138, id139, id140, id141, id142,
    id143, id144, id145, id146, id147, id148, id149, id150, id151, id152, id153, id154, id155, id156, id157, id158, id159, id160, id161, id162, id163, id164, id165, id166,





    //itembonus
    idBonus1,
    idBonus2,
    idBonus3,
    idBonus4,
    idBonus5,
    idBonus6,
    idBonus7,
    idBonus8,
    idBonus9,
    idBonus10,
    idBonus11,
    idBonus12,
    idBonus13,
    idBonus14,
    idBonus15,
    idBonus16,
    idBonus17,
    idBonus18,
    idBonus19,
    idBonus20,
    idBonus21,
    idBonus22,
    idBonus23,
    idBonus24,
    idBonus25,
    idBonus26,
    idBonus27,
    idBonus28,
    idBonus29,
    idBonus30,
    idBonus31,
    idBonus32,
    idBonus33,
    idBonus34,
    idBonus35,
    idBonus36,
    idBonus37,
    idBonus38,
    idBonus39,
    idBonus40,
    idBonus41,
    idBonus42,
    idBonus43,
    idBonus44,
    idBonus45,
    idBonus46,
    idBonus47,
    idBonus48,
    idBonus49,
    idBonus50,



}

public enum Scene
{
    Form_Loading,
    Home,
    Main,
    LevelBonus,
}
public enum ThemeId
{
    animal,
    food,
    music,
    school,
    vehicle,
}
