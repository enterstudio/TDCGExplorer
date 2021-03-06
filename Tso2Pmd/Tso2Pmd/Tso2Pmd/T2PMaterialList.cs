﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using TDCG;
using TDCGUtils;
using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.struct_type.pmd;

namespace Tso2Pmd
{
    class T2PMaterialList
    {
        public List<string> name_list = new List<string>();
        public List<PMD_Material> material_list = new List<PMD_Material>();

        List<TSOFile> TSOList = new List<TSOFile>();
        List<string> cateList = new List<string>();
        T2PTextureList tex_list = new T2PTextureList();
        List<string> toon_name_list = new List<string>();

        public T2PMaterialList(List<TSOFile> TSOList, List<string> cateList)
        {
            this.TSOList = TSOList;
            this.cateList = cateList;

            // テクスチャの準備
            for (int tso_num = 0; tso_num < TSOList.Count; tso_num++)
            foreach (TSOTex tex in TSOList[tso_num].textures)
            {
                tex_list.Add(tex, tso_num);
            }
        }

        public void Save(string path, string file_name, bool spheremap_flag)
        {
            // -----------------------------------------------------
            // テクスチャをBitmapファイルに出力
            tex_list.Save(path, spheremap_flag);

            // -----------------------------------------------------
            // マテリアル名のリストが書かれたファイルを出力
            // ファイルを開く
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                path + "/" + file_name + ".txt",
                false,
                System.Text.Encoding.GetEncoding("shift_jis"));

            // 書き出す
            foreach (string name in name_list)
                sw.WriteLine(name);

            //閉じる
            sw.Close();
        }
        
        // TSOSubScriptより、PMD_Materialを生成する
        // (ただし、頂点インデックス数は0となっているため、後に設定する必要がある)
        public void Add(int tso_num, int script_num, bool edge, bool spheremap_flag)
        {
            PMD_Material pmd_m = new PMD_Material();

            // スクリプトよりシェーダパラメータを取得
            Shader shader = new Shader();
            shader.Load(TSOList[tso_num].sub_scripts[script_num].lines);

            pmd_m.col4Diffuse = new MmdColor4(1.0f, 1.0f, 1.0f, 1.0f);
            pmd_m.fShininess = 6.0f;
            pmd_m.col3Specular = new MmdColor3(0.15f, 0.15f, 0.15f);
            pmd_m.col3Ambient = new MmdColor3(0.5f, 0.5f, 0.5f);

            if (edge == true) pmd_m.edge_flag = 1;
            else pmd_m.edge_flag = 0;

            // 頂点インデックス数（0となっているため、後に設定する必要がある）
            pmd_m.ulNumIndices = 0;

            // colorテクスチャ
            pmd_m.szTextureFileName = tex_list.GetFileName(tso_num, shader.ColorTexName);

            // toonテクスチャ
            string toon_file = tex_list.GetFileName(tso_num, shader.ShadeTexName);
            if (toon_file != null) // 存在しないtoonテクスチャを参照しているパーツがあるのでこれを確認
            {
                if (toon_name_list.IndexOf(toon_file) != -1)
                {
                    // toonテクスチャファイル中でのインデックス
                    pmd_m.toon_index = toon_name_list.IndexOf(toon_file);
                }
                else
                {
                    if (toon_name_list.Count <= 9)
                    {
                        // toonテクスチャとするテクスチャ名を記憶
                        // 後にtoonテクスチャファイル名を出力するときに使用。
                        toon_name_list.Add(toon_file);

                        // toonテクスチャファイル中でのインデックス
                        pmd_m.toon_index = toon_name_list.Count - 1;
                    }
                    else
                    {
                        // toonテクスチャとするテクスチャ名を記憶
                        // 後にtoonテクスチャファイル名を出力するときに使用。
                        toon_name_list.Add("toon10.bmp"); // 10以上は無理なので、それ以上は全てtoon10.bmp

                        // toonテクスチャファイル中でのインデックス
                        pmd_m.toon_index = 9; // 10以上は無理なので、それ以上は全て9
                    }
                }

                // スフィアマップ
                if (spheremap_flag == true)
                {
                    // toonテクスチャが256×16のサイズなら、スフィアマップを指定する
                    Bitmap toon_bmp = tex_list.GetBitmap(tso_num, shader.ShadeTexName);
                    if (toon_bmp.Width == 256 && toon_bmp.Height == 16)
                    {
                        string sphere_file
                            = System.Text.RegularExpressions.Regex.Replace(toon_file, ".bmp", ".sph");
                        pmd_m.szTextureFileName
                            = pmd_m.szTextureFileName + "*" + sphere_file;
                    }
                }
            }
            else
            {
                pmd_m.toon_index = 9;
            }

            // 要素を追加
            name_list.Add(cateList[tso_num] + " " 
                    + TSOList[tso_num].sub_scripts[script_num].Name);
            material_list.Add(pmd_m);
        }

        // 隣り合う同一のマテリアルを統合する
        public void MergeMaterials()
        {
            for (int i = 0; i < material_list.Count - 1; i++)
            {
                if (EqualMaterial(material_list[i], material_list[i+1]) == 0)
                {
                    material_list[i].ulNumIndices += material_list[i+1].ulNumIndices;
                    material_list.RemoveAt(i+1);
                    name_list.RemoveAt(i+1);
                    i = 0;
                }
            }
        }

        // ２つのマテリアルが等しいか判定する
        public int EqualMaterial(PMD_Material m1, PMD_Material m2)
        {
            // edge_flag
            if (m1.edge_flag != m2.edge_flag) return -1;

            // colorテクスチャ
            if (m1.szTextureFileName != m2.szTextureFileName) return -1;

            // toonテクスチャ
            if (m1.toon_index != m2.toon_index) return -1;

            return 0;
        }

        // トゥーンテクスチャファイル名を得る
        public string[] GetToonFileNameList()
        {
            string[] name_list = new string[10];

            if (toon_name_list.Count <= 10)
            {
                for (int i = 0; i < toon_name_list.Count; i++)
                {
                    name_list[i] = toon_name_list[i];
                }
                for (int i = toon_name_list.Count; i < 10; i++)
                {
                    name_list[i] = "toon" + i.ToString("00") + ".bmp";
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    name_list[i] = toon_name_list[i];
                }
            }

            // toonテクスチャがうまく呼び出せない場合に呼び出す、空のtoonテクスチャ
            name_list[9] = "toon10.bmp";

            return name_list;
        }
    }
}
