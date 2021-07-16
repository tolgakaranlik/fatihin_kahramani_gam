using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MagicShortcut
{
    public int Key
    {
        get
        {
            return key;
        }
    }

    public int Code
    {
        get
        {
            return code;
        }
    }

    public Sprite Icon
    {
        get
        {
            return icon;
        }
    }

    private int key;
    private int code;
    private Sprite icon;

    public MagicShortcut(int key, int code, Sprite icon)
    {
        this.key = key;
        this.code = code;
        this.icon = icon;
    }
}

