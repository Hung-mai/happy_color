using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NameUtils
{
    public static List<string> NameList = new List<string>()
    {
        "Victor",
        "Kenneth",
        "Parker",
        "Bobby",
        "James",
        "Walter",
        "Clark",
        "Cooper",
        "Perez",
        "Alexander",
        "Rogers",
        "Morris",
        "Campbell",
        "Garcia",
        "Carol",
        "Lawrence",
        "Hughes",
        "Beverly",
        "Anderson",
        "Daniel",
        "Adams",
        "Catherine",
        "Marie",
        "Bennett",
        "Carolyn",
        "Patterson",
        "Harris",
        "Bonnie",
        "Johnny",
        "Moore",
        "Marilyn",
        "Chris",
        "Powell",
        "Gonzales",
        "Wilson",
        "Lopez",
        "Murphy",
        "Gregory",
        "Martinez",
        "Carter",
        "Coleman",
        "Mitchell",
        "Carlos ",
        "Edwards",
        "Kathleen",
        "Howard",
        "Christine",
        "Torres",
        "Jose",
        "Christina",
        "Davis",
        "Virginia",
        "Paula",
        "Louise",
        "Amanda",
        "Judy",
        "Philip",
        "Coleman",
        "Campbell",
        "Hernandez",
        "Mitchell",
        "Lori",
        "Kathy",
        "Diane",
        "Emily",
        "Shirley",
        "Miller",
        "Rogers",
        "Powell",
        "Rivera",
        "Bonnie",
        "Craig",
        "Michelle",
        "Laura",
        "Flores",
        "Brooks",
        "Richardson",
        "Walker"
    };

    public static List<string> GetUniqueNameList(int number)
    {
        var list = NameList.OrderBy(d => System.Guid.NewGuid());

        return list.Take(number).ToList();
    }
   
}
