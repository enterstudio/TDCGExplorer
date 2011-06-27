﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCGUtils.Extensions;

namespace TDCGUtils
{
    public class PmdFile
    {
        public PMD_Header pmd_header; // ヘッダー情報

        // 頂点数
        public int number_of_vertex
        {
            get { return vertices.Length; }
        }
        public PMD_Vertex[] vertices; // 頂点配列

        // 頂点インデックス数
        public int number_of_indices
        {
            get { return vindices.Length; }
        }
        public short[] vindices; // 頂点インデックス配列

        // マテリアル数　
        public int number_of_materials
        {
            get { return materials.Length; }
        }
        public PMD_Material[] materials; // マテリアル配列

        // Bone数
        public int number_of_bone
        {
            get { return nodes.Length; }
        }
        public PMD_Bone[] nodes; // Bone配列

        // IK数
        public int number_of_ik
        {
            get { return pmd_ik.Length; }
        }
        public PMD_IK[] pmd_ik; // IK配列

        // 表情数
        public int number_of_skin
        {
            get { return skins.Length; }
        }
        public PMD_Skin[] skins; // Face配列

        // 表情枠に表示する表情数
        public int skin_disp_count
        {
            get { return skin_disp_indices.Length; }
        }
        public int[] skin_disp_indices; // 表情番号

        // ボーン枠用の枠名数
        public int bone_disp_name_count
        {
            get { return disp_names.Length; }
        }
        public string[] disp_names; // 枠名(50Bytes/枠)

        // ボーン枠に表示するボーン数 (枠0(センター)を除く、すべてのボーン枠の合計)
        public int bone_disp_count
        {
            get { return bone_disps.Length; }
        }
        public PMD_BoneDisp[] bone_disps; // 枠用ボーンデータ (3Bytes/bone)

        public int english_name_compatibility; // 英名対応(01:英名対応あり)

        public string[] toon_file_name;//[100][10]; // トゥーンテクスチャファイル名

        // 剛体数 // 2D 00 00 00 == 45
        public int rbody_count
        {
            get { return bodies.Length; }
        }
        public PMD_RBody[] bodies;// 剛体データ(83Bytes/rigidbody)

        // ジョイント数 // 1B 00 00 00 == 27
        public int joint_count
        {
            get { return joints.Length; }
        }
        public PMD_Joint[] joints;// ジョイントデータ(124Bytes/joint)

        /// <summary>
        /// 指定パスに保存します。
        /// </summary>
        /// <param name="dest_file">パス</param>
        public void Save(string dest_file)
        {
            using (Stream dest_stream = File.Create(dest_file))
                Save(dest_stream);
        }

        /// <summary>
        /// 指定ストリームに保存します。
        /// </summary>
        /// <param name="dest_stream">ストリーム</param>
        public void Save(Stream dest_stream)
        {
            BinaryWriter bw = new BinaryWriter(dest_stream);

            // -----------------------------------------------------
            // ヘッダー情報
            pmd_header.Write(bw);

            // -----------------------------------------------------
            // 頂点数
            bw.Write((int)number_of_vertex);

            // ボーン名をIDに置き換え
            foreach (PMD_Vertex vertex in vertices)
            {
                if (vertex.unBoneName[0] == null)
                    vertex.bone_indices[0] = -1;
                else
                    vertex.bone_indices[0] = GetBoneIDByName(vertex.unBoneName[0]);
                
                if (vertex.unBoneName[1] == null)
                    vertex.bone_indices[1] = -1;
                else
                    vertex.bone_indices[1] = GetBoneIDByName(vertex.unBoneName[1]);
            }

            // 頂点配列
            for (int i = 0; i < number_of_vertex; i++)
            {
                vertices[i].Write(bw);
            }

            // -----------------------------------------------------
            // 頂点インデックス数
            bw.Write((int)number_of_indices);

            // 頂点インデックス配列
            for (int i = 0; i < number_of_indices; i++)
            {
                bw.Write((short)vindices[i]);
            }

            // -----------------------------------------------------
            // マテリアル数
            bw.Write((int)number_of_materials);

            // マテリアル配列
            for (int i = 0; i < number_of_materials; i++)
            {
                materials[i].Write(bw);
            }

            // -----------------------------------------------------
            // Bone数
            bw.Write((short)number_of_bone);

            // ボーン名をIDに置き換え
            foreach (PMD_Bone bone in nodes)
            {
                if (bone.ParentName == null)
                    bone.nParentNo = -1;
                else
                    bone.nParentNo = GetBoneIDByName(bone.ParentName);
                
                if (bone.TailName == null)
                    bone.nTailNo = 0;
                else
                    bone.nTailNo = GetBoneIDByName(bone.TailName);
                
                if (bone.IKTargetName == null)
                    bone.unIKTarget = 0;
                else
                    bone.unIKTarget = GetBoneIDByName(bone.IKTargetName);
            }

            // Bone配列
            for (int i = 0; i < number_of_bone; i++)
            {
                nodes[i].Write(bw);
            }

            // -----------------------------------------------------
            // IK数
            bw.Write((short)number_of_ik);

            if (number_of_ik > 0)
            {
                // ボーン名をIDに置き換え
                foreach (PMD_IK ik in pmd_ik)
                {
                    if (ik.target_node_name == null)
                        ik.nTargetNo = -1;
                    else
                        ik.nTargetNo = GetBoneIDByName(ik.target_node_name);
                    
                    if (ik.effector_node_name == null)
                        ik.nEffNo = -1;
                    else
                        ik.nEffNo = GetBoneIDByName(ik.effector_node_name);

                    if (ik.nTargetNo <= -1)
                        ik.target_node_name = null;
                    else
                        ik.target_node_name = nodes[ik.nTargetNo].name;
                    
                    if (ik.nEffNo <= -1)
                        ik.effector_node_name = null;
                    else
                        ik.effector_node_name = nodes[ik.nEffNo].name;

                    ik.chain_node_ids = new int[ik.chain_node_names.Count];
                    for (int i = 0; i < ik.chain_length; i++)
                    {
                        if (ik.chain_node_names[i] == null)
                            ik.chain_node_ids[i] = -1;
                        else
                            ik.chain_node_ids[i] = GetBoneIDByName(ik.chain_node_names[i]);
                    }
                }

                // IK配列
                for (int i = 0; i < number_of_ik; i++)
                    pmd_ik[i].Write(bw);
            }

            // -----------------------------------------------------
            // Face数
            bw.Write((short)number_of_skin);

            // Face配列
            for (int i = 0; i < number_of_skin; i++)
            {
                skins[i].Write(bw);
            }

            // -----------------------------------------------------
            // 表情枠
            // -----------------------------------------------------
            // 表情枠に表示する表情数
            bw.Write((byte)skin_disp_count);

            // 表情番号
            for (int i = 0; i < skin_disp_count; i++)
            {
                bw.Write((short)skin_disp_indices[i]);
            }

            // -----------------------------------------------------
            // ボーン枠
            // -----------------------------------------------------
            // ボーン枠用の枠名数
            bw.Write((byte)bone_disp_name_count);

            // 枠名(50Bytes/枠)
            for (int i = 0; i < bone_disp_name_count; i++)
            {
                bw.WriteCString(disp_names[i], 50);
            }

            // -----------------------------------------------------
            // ボーン枠に表示するボーン
            // -----------------------------------------------------
            // ボーン枠に表示するボーン数
            bw.Write((int)bone_disp_count);

            // ボーン名をIDに置き換え
            for (int i = 0; i < bone_disps.Length; i++)
            {
                bone_disps[i].bone_index = GetBoneIDByName(bone_disps[i].bone_name);
            }

            // 枠用ボーンデータ (3Bytes/bone)
            for (int i = 0; i < bone_disp_count; i++)
            {
                bone_disps[i].Write(bw);
            }

            // -----------------------------------------------------
            // 英名対応(0:英名対応なし, 1:英名対応あり)
            // -----------------------------------------------------
            bw.Write((byte)0);//english_name_compatibility

            // -----------------------------------------------------
            // トゥーンテクスチャファイル名
            // -----------------------------------------------------
            for (int i = 0; i < 10; i++)
            {
                bw.WriteCString(toon_file_name[i], 100);
            }

            // -----------------------------------------------------
            // 剛体
            // -----------------------------------------------------
            // 剛体数
            bw.Write((int)rbody_count);

            // 剛体データ(83Bytes/rigidbody)
            for (int i = 0; i < rbody_count; i++)
            {
                bodies[i].Write(bw);
            }

            // -----------------------------------------------------
            // ジョイント
            // -----------------------------------------------------
            // ジョイント数
            bw.Write((int)joint_count);

            for (int i = 0; i < joint_count; i++)
            {
                joints[i].Write(bw);
            }
        }

        public PMD_Bone GetBoneByName(string name)
        {
            foreach (PMD_Bone bone in nodes)
            {
                if (bone.name == name)
                    return bone;
            }
            return null;
        }

        short GetBoneIDByName(string name)
        {
            for (short i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].name == name)
                    return i;
            }
            return -1;
        }
    }

    public class PMD_Header
    {
        public const int SIZE_OF_STRUCT = 3 + 4 + 20 + 256;
        public String magic;
        public float version;
        public String name;
        public String comment;

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.magic, 3);
            writer.Write(this.version);
            writer.WriteCString(this.name, 20);
            writer.WriteCString(this.comment, 256);
        }
    }

    public class PMD_Vertex
    {
        // 座標
        public Vector3 position = Vector3.Empty;

        // 法線ベクトル
        public Vector3 normal = Vector3.Empty;

        // テクスチャ座標
        public float u, v;

        // ボーン番号
        internal int[] bone_indices = new int[2];

        // ブレンドの重み (0～100％)
        public sbyte weight;	

        // エッジフラグ
        public sbyte edge;	

        public string[] unBoneName = new string[2];	// ボーン番号

        internal void Write(BinaryWriter writer)
        {
            writer.Write(ref this.position);
            writer.Write(ref this.normal);
            writer.Write(u);
            writer.Write(v);
            writer.Write((ushort)this.bone_indices[0]);
            writer.Write((ushort)this.bone_indices[1]);
            writer.Write(this.weight);
            writer.Write(this.edge);
        }
    }

    public class PMD_Material
    {
        public Vector4 diffuse = Vector4.Empty;
        public float shininess;
        public Vector3 specular = Vector3.Empty;
        public Vector3 ambient = Vector3.Empty;
        public sbyte toon_tex_id; // toon??.bmp // 0.bmp:0xFF, 1(01).bmp:0x00 ・・・ 10.bmp:0x09
        public sbyte edge; // 輪郭、影
        public int vindices_count;		// この材質に対応する頂点数
        public String tex_path;	// テクスチャファイル名

        internal void Write(BinaryWriter writer)
        {
            writer.Write(ref this.diffuse);
            writer.Write(this.shininess);
            writer.Write(ref this.specular);
            writer.Write(ref this.ambient);
            writer.Write(this.toon_tex_id);
            writer.Write(this.edge);
            writer.Write(this.vindices_count);
            writer.WriteCString(this.tex_path, 20);
        }
    }

    public class PMD_Bone
    {
        // ボーン名 (0x00 終端，余白は 0xFD)
        public String name;

        // 親ボーン番号 (なければ -1)
        internal int nParentNo;

        // 子ボーン番号
        internal int nTailNo;

        // ボーンの種類
        public int kind;

        // IK時のターゲットボーン
        internal int unIKTarget;

        // モデル原点からの位置
        public Vector3 position = Vector3.Empty;

        // 親ボーン名
        public string ParentName;

        // 子ボーン名
        public string TailName;

        // IK時のターゲットボーン
        public string IKTargetName;

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.name, 20);
            writer.Write((short)this.nParentNo);
            writer.Write((short)this.nTailNo);
            writer.Write((byte)this.kind);
            writer.Write((short)this.unIKTarget);
            writer.Write(ref this.position);
        }
    }

    public class PMD_IK
    {
        // IKターゲットボーン番号
        internal int nTargetNo;
        
        // IK先端ボーン番号
        internal int nEffNo;
        
        // IKを構成するボーンの数
        public int chain_length
        {
            get { return chain_node_ids.Length; }
        }
        
        public int niteration;
        
        public float weight;
        
        // IKを構成するボーンの配列
        internal int[] chain_node_ids;
        
        // IKターゲットボーン名
        public string target_node_name;
        
        // IK先端ボーン名
        public string effector_node_name;
        
        // IKを構成するボーンの配列
        public List<string> chain_node_names = new List<string>();
        
        internal void Write(BinaryWriter writer)
        {
            writer.Write((short)this.nTargetNo);
            writer.Write((short)this.nEffNo);
            writer.Write((sbyte)this.chain_length);
            writer.Write((ushort)this.niteration);
            writer.Write(this.weight);

            for (int i = 0; i < this.chain_length; i++)
            {
                writer.Write((ushort)this.chain_node_ids[i]);
            }
        }
    }

    public class PMD_Skin
    {
        // 表情名 (0x00 終端，余白は 0xFD)
        public String name;

        // 表情頂点数
        int vertices_count;
        
        // 分類 (0：base、1：まゆ、2：目、3：リップ、4：その他)
        public int panel_id;

        // 表情頂点データ
        public PMD_SkinVertex[] vertices = PMD_SkinVertex.createArray(64);

        public PMD_Skin(int n)
        {
            this.vertices_count = n;

            if (this.vertices_count > this.vertices.Length)
            {
                this.vertices = PMD_SkinVertex.createArray(this.vertices_count);
            }
        }

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.name, 20);
            writer.Write(this.vertices_count);
            writer.Write((sbyte)this.panel_id);

            for (int i = 0; i < this.vertices_count; i++)
            {
                this.vertices[i].Write(writer);
            }
        }
    }

    public class PMD_SkinVertex
    {
        public int vertex_id;
        public Vector3 position = Vector3.Empty;

        public static PMD_SkinVertex[] createArray(int i_length)
        {
            PMD_SkinVertex[] ret = new PMD_SkinVertex[i_length];
            for (int i = 0; i < i_length; i++)
            {
                ret[i] = new PMD_SkinVertex();
            }
            return ret;
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(this.vertex_id);
            writer.Write(ref this.position);
        }
    }

    public class PMD_BoneDisp
    {
        // 枠用ボーン番号
        internal short bone_index;
        
        // 表示枠番号
        public sbyte disp_group_id; 

        public string bone_name;

        internal void Write(BinaryWriter writer)
        {
            writer.Write(bone_index);
            writer.Write(disp_group_id);
        }
    }

    public class PMD_RBody
    {
        public String name; // 諸データ：名称 // 頭
        public int rel_bone_id; // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
        public int group_id; // 諸データ：グループ // 00
        public int group_non_collision; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
        public int shape_id; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
        
        // 形状：半径(幅) // CD CC CC 3F // 1.6
        // 形状：高さ // CD CC CC 3D // 0.1
        // 形状：奥行 // CD CC CC 3D // 0.1
        public Vector3 size = Vector3.Empty;
        
        public Vector3 position = Vector3.Empty; // 位置：位置(x, y, z)
        public Vector3 rotation = Vector3.Empty; // 位置：回転(rad(x), rad(y), rad(z))
        public float weight; // 諸データ：質量 // 00 00 80 3F // 1.0
        public float position_dim; // 諸データ：移動減 // 00 00 00 00
        public float rotation_dim; // 諸データ：回転減 // 00 00 00 00
        public float recoil; // 諸データ：反発力 // 00 00 00 00
        public float friction; // 諸データ：摩擦力 // 00 00 00 00
        public int type; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.name, 20);
            writer.Write((short)this.rel_bone_id);
            writer.Write((byte)this.group_id);
            writer.Write((short)this.group_non_collision);
            writer.Write((byte)this.shape_id);
            writer.Write(ref this.size);
            writer.Write(ref this.position);
            writer.Write(ref this.rotation);
            writer.Write(this.weight);
            writer.Write(this.position_dim);
            writer.Write(this.rotation_dim);
            writer.Write(this.recoil);
            writer.Write(this.friction);
            writer.Write((byte)this.type);
        }
    }

    public class PMD_Joint
    {
        public String name; // 諸データ：名称 // 右髪1
        public int rbody_a_id; // 諸データ：剛体A
        public int rbody_b_id; // 諸データ：剛体B
        public Vector3 position = Vector3.Empty; // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
        public Vector3 rotation = Vector3.Empty; // 諸データ：回転(rad(x), rad(y), rad(z))
        public Vector3 position_min = Vector3.Empty; // 制限：移動1(x, y, z)
        public Vector3 position_max = Vector3.Empty; // 制限：移動2(x, y, z)
        public Vector3 rotation_min = Vector3.Empty; // 制限：回転1(rad(x), rad(y), rad(z))
        public Vector3 rotation_max = Vector3.Empty; // 制限：回転2(rad(x), rad(y), rad(z))
        public Vector3 spring_position = Vector3.Empty; // ばね：移動(x, y, z)
        public Vector3 spring_rotation = Vector3.Empty; // ばね：回転(rad(x), rad(y), rad(z))

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.name, 20);
            writer.Write(this.rbody_a_id);
            writer.Write(this.rbody_b_id);
            writer.Write(ref this.position);
            writer.Write(ref this.rotation);
            writer.Write(ref this.position_min);
            writer.Write(ref this.position_max);
            writer.Write(ref this.rotation_min);
            writer.Write(ref this.rotation_max);
            writer.Write(ref this.spring_position);
            writer.Write(ref this.spring_rotation);
        }
    }
}
