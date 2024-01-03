using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class HopfieldDataBuilder
{
    public HopfieldDataBuilder(Vector2 size) { 
        m_size = size;
    }
    public Vector2 Size => m_size;
    Vector2 m_size;
    public List<Data> Datas = new();
    
    public void AddEmpty()
    {
        Datas.Add(new Data(Size));
    }

    public static HopfieldDataBuilder FromString(string data)
    {
        var datas = data.Split("\n\n", System.StringSplitOptions.RemoveEmptyEntries);
        var result = new HopfieldDataBuilder(Data.FromString(datas[0]).Size);
        foreach (var str in datas)
            result.Datas.Add(Data.FromString(str));
        return result;
    }

    public class Data
    {
        public Vector2 Size => m_size;
        Vector2 m_size;
        bool[,] data;
        public bool[,] Datas => data;
        public bool this[Vector2Int pos]
        {
            get => this[pos.x, pos.y];
            set => this[pos.x, pos.y] = value;
        }
        public bool this[int x, int y]
        {
            get => data[y, x];
            set
            {
                data[y, x] = value;
                EditView[y][x].style.backgroundColor = value ? DocStyle.Current.FrontgroundColor : DocStyle.Current.SubBackgroundColor;
                View[y][x].style.backgroundColor = value ? DocStyle.Current.FrontgroundColor : DocStyle.Current.SubBackgroundColor;
            }
        }
        VisualElement m_editView, m_view;
        public Data(Vector2 size)
        {
            m_size = size;
            data = new bool[(int)size.y, (int)size.x];
        }
        public Data(bool[,] datas)
        {
            m_size = new Vector2(datas.GetLength(1), datas.GetLength(0));
            data = new bool[(int)m_size.y, (int)m_size.x];
            for(int y = 0; y < m_size.y; y++)
            {
                for(int x = 0; x < m_size.x; x++)
                {
                    this[x,y] = datas[y, x];
                }
            }
        }
        public void ResizeViewLayout(float maxSizePx, float spacingPx)
        {
            ResizeLayout(maxSizePx, spacingPx, View);
        }
        public void ResizeEditLayout(float maxSizePx, float spacingPx)
        {
            ResizeLayout(maxSizePx, spacingPx, EditView);
        }
        public void ResizeLayout(float maxSizePx, float spacingPx, VisualElement target)
        {
            target.style.width = maxSizePx;
            target.style.height = maxSizePx;
            float size;
            if (Size.x > Size.y)
                size = (maxSizePx + spacingPx) / Size.x - spacingPx;
            else
                size = (maxSizePx + spacingPx) / Size.y - spacingPx;
            spacingPx /= 2f;
            foreach (var row in target.Children())
            {
                row.style.height = size;
                row.style.marginTop = spacingPx;
                row.style.marginBottom = spacingPx;
                foreach (var unit in row.Children())
                {
                    unit.style.width = size;
                    unit.style.marginLeft = spacingPx;
                    unit.style.marginRight = spacingPx;
                }
            }
        }
        public VisualElement View
        {
            get
            {
                if (m_view == null)
                    _ = EditView;
                return m_view;
            }
        }
        public VisualElement EditView
        {
            get
            {
                if(m_editView == null)
                {
                    m_editView = new();
                    m_view = new();
                    for (int y = 0; y < Size.y; y++)
                    {
                        VisualElement row = new VisualElement();
                        VisualElement view_row = new VisualElement();
                        row.style.flexDirection = FlexDirection.Row;
                        row.style.justifyContent = Justify.Center;
                        view_row.style.flexDirection = FlexDirection.Row;
                        m_editView.Add(row);
                        m_view.Add(view_row);
                        for(int x = 0; x < Size.x; x++)
                        {
                            VisualElement unit = new VisualElement();
                            VisualElement view_unit = new VisualElement();
                            Vector2Int mPos = new Vector2Int(x, y);
                            Action draw = () =>
                            {
                                bool isData;
                                if (Input.GetKey(KeyCode.Mouse0))
                                    isData = true;
                                else if (Input.GetKey(KeyCode.Mouse1))
                                    isData = false;
                                else
                                    return;

                                this[mPos] = isData;
                            };
                            unit.RegisterCallback<PointerMoveEvent>(evt => { draw(); });
                            unit.RegisterCallback<PointerDownEvent>(evt => { draw(); });
                            unit.style.backgroundColor = this[mPos] ? DocStyle.Current.FrontgroundColor : DocStyle.Current.SubBackgroundColor;
                            view_unit.style.backgroundColor = this[mPos] ? DocStyle.Current.FrontgroundColor : DocStyle.Current.SubBackgroundColor;
                            row.Add(unit);
                            view_row.Add(view_unit);
                        }
                    }
                }
                return m_editView;
            }
        }
        public static Data FromString(string data)
        {
            var rowData = data.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
            Data result = new Data(new Vector2(rowData[0].Length, rowData.Length));
            for(int y = 0; y < rowData.Length; y++)
            {
                for(int x=0,xmax = rowData[0].Length; x < xmax; x++)
                {
                    result[x, y] = rowData[y][x] == '1';
                }
            }
            return result;
        }
    }
}
