using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class columnItem
{
    public List<Item> items;
}
public class Level : MonoBehaviour
{
    [Header("----level chuyen theme---------")]
    public bool isLevelTranportTheme = false;
    public ThemeId themeTransport;

    public Transform[] containerItems;
    [Header("Option*")]
    public Collider boundary;
    public Item[] itemsTut;

    [Header("-------listItemInColumn-------")]
    public List<columnItem> columnItems;

    [Header("trigger change item")]
    public bool isSign = false;

    public bool isDrawGizmo = false;

    private void OnValidate()
    {
        if (!isSign) return;
        foreach (columnItem ci in columnItems)
        {
            ci.items.Clear();
        }
        for (int y = 0; y < containerItems.Length; y++)
        {
            for (int i = 0; i < containerItems[y].childCount; i++)
            {
                columnItems[y].items.Add(containerItems[y].GetChild(i).GetComponent<Item>());
            }
        }
        isSign = false;
    }
    private void OnDrawGizmos()
    {
        if (!isDrawGizmo) return;
        for (int i = 0; i < containerItems.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(containerItems[i].position, containerItems[i].position + Vector3.up * 10000f);
        }
    }
    [ContextMenu("CheckAmountItem")]
    public void CheckAmountItem()
    {
        List<Item> items = new List<Item>();
        foreach (columnItem c in columnItems)
        {
            foreach (Item i in c.items)
            {
                items.Add(i);
            }
        }
        CheckAmount(items);
    }
    public static void CheckAmount(List<Item> items)
    {
        List<ItemType> itemtypes = new List<ItemType>();
        int sumObject = 0;
        for (int i = 0; i < items.Count; i++)
        {
            bool isType = false;
            string nameId = "";
            int count = 0;
            foreach (ItemType it in itemtypes)
            {
                if (items[i].type == it)
                {
                    isType = true;
                    break;
                }
            }
            if (isType)
            {
                continue;
            }
            else
            {
                itemtypes.Add(items[i].type);
                nameId = items[i].type.ToString() + " " + items[i].gameObject.name;
                foreach (Item it in items)
                {
                    if (it.type == items[i].type)
                    {
                        count += 1;
                        sumObject += 1;
                    }
                }
                Debug.Log(nameId + " : " + count + " Object in level");
            }
        }
        Debug.Log("SummaryObject : " + sumObject);
    }
    [Header("---------New Mechanic-----------")]
    public List<Item> itemsInLevel;
    public Template[] templates;
    [ContextMenu("Arrange")]
    public void ArrangeTemplate()
    {
        for(int i = 1; i < templates.Length; i++)
        {
            templates[i].transform.position = new Vector3(templates[i].transform.position.x, i * templates[i - 1].y * templates[i - 1].heightItem, templates[i].transform.position.z);
                //Vector3.up * i * templates[i - 1].y * templates[i - 1].heightItem;

        }
    }
    [ContextMenu("InitLevel")]
    public void InitLevel()
    {
        itemsInLevel.Shuffle();
        foreach (Template t in templates)
        {
            List<Item> temp = new List<Item>();
            for (int i = 0; i <t.countId; i++)
            {
                temp.Add(itemsInLevel[i]);
            }
            itemsInLevel.RemoveRange(0,t.countId);
            t.FillItem(temp);
        }
    }
}
