﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TDCG;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCGExplorer
{
    public class TSOCameraAutoCenter
    {
        Viewer viewer;

        public TSOCameraAutoCenter(Viewer TsoView)
        {
            viewer = TsoView;
        }

        internal char current_row = 'A';

        private void SetCurrentTSOFileName(string filename)
        {
            string basename = Path.GetFileNameWithoutExtension(filename);
            if (basename.Length == 12)
                current_row = basename.ToUpper()[9];
            else
                current_row = 'A';
        }

        private int GetCenterBoneType()
        {
            switch (current_row)
            {
                case 'S'://手首
                case 'Z'://手持ちの小物
                    return 1;
                case 'W'://タイツ・ガーター
                case 'I'://靴下
                    return 2;
                case 'O'://靴
                    return 3;
                case 'X'://腕装備(手甲など)
                    return 4;
                default:
                    return 0;
            }
        }

        private string GetCenterBoneName()
        {
            switch (current_row)
            {
                case 'A'://身体
                    return "W_Neck";
                case 'E'://瞳
                case 'D'://頭皮(生え際)
                case 'B'://前髪
                case 'C'://後髪
                case 'U'://アホ毛類
                    return "Kami_Oya";
                case 'Q'://眼鏡
                case 'V'://眼帯
                case 'Y'://リボン
                case 'P'://頭部装備(帽子等)
                    return "face_oya";
                case '3'://イヤリング類
                case '0'://眉毛
                case '2'://ほくろ
                case '1'://八重歯
                    return "Head";
                case 'R'://首輪
                    return "W_Neck";
                case 'F'://ブラ
                case 'J'://上衣(シャツ等)
                case 'T'://背中(羽など)
                case 'L'://上着オプション(エプロン等)
                    return "W_Spine3";
                case 'G'://全身下着・水着
                case 'K'://全身衣装(ナース服等)
                    return "W_Spine1";
                case 'S'://手首
                case 'X'://腕装備(手甲など)
                case 'Z'://手持ちの小物
                    return "W_Hips";//not reached
                case 'H'://パンツ
                case 'M'://下衣(スカート等)
                case 'N'://尻尾
                    return "W_Hips";
                case 'W'://タイツ・ガーター
                case 'I'://靴下
                    return "W_Hips";//not reached
                case 'O'://靴
                    return "W_Hips";//not reached
                default:
                    return "W_Hips";
            }
        }

        public void UpdateCenterPosition(string tsoname)
        {
            Vector3 position = new Vector3(0, 0, 0);

            TMOFile tmo = viewer.FigureList[0].Tmo;

            Dictionary<string, TMONode> nodemap = new Dictionary<string, TMONode>();
            foreach (TMONode node in tmo.nodes)
            {
                if (nodemap.ContainsKey(node.Name) == false)
                    nodemap.Add(node.Name, node);
            }

            SetCurrentTSOFileName(tsoname);

            switch (GetCenterBoneType())
            {
                case 1://Hand
                    {
#if false
                        TMONode tmo_nodeR;
                        TMONode tmo_nodeL;
                        string boneR = "W_RightHand";
                        string boneL = "W_LeftHand";
                        if (nodemap.TryGetValue(boneR, out tmo_nodeR) && nodemap.TryGetValue(boneL, out tmo_nodeL))
                        {
                            Matrix mR = tmo_nodeR.combined_matrix;
                            Matrix mL = tmo_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, (mR.M43 + mL.M43) / 2.0f);
                        }
#else
                        // 両手はあまりにも離れているので右手にセンターを置く
                        TMONode tmo_nodeR;
                        string boneR = "W_RightHand";
                        if (nodemap.TryGetValue(boneR, out tmo_nodeR))
                        {
                            Matrix mR = tmo_nodeR.combined_matrix;
                            position = new Vector3(mR.M41, mR.M42, mR.M43);
                        }
#endif
                    }
                    break;
                case 2://Leg
                    {
                        TMONode tmo_nodeR;
                        TMONode tmo_nodeL;
                        string boneR = "W_RightLeg";
                        string boneL = "W_LeftLeg";
                        if (nodemap.TryGetValue(boneR, out tmo_nodeR) && nodemap.TryGetValue(boneL, out tmo_nodeL))
                        {
                            Matrix mR = tmo_nodeR.combined_matrix;
                            Matrix mL = tmo_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, (mR.M43 + mL.M43) / 2.0f);
                        }
                    }
                    break;
                case 3://Foot
                    {
                        TMONode tmo_nodeR;
                        TMONode tmo_nodeL;
                        string boneR = "W_RightFoot";
                        string boneL = "W_LeftFoot";
                        if (nodemap.TryGetValue(boneR, out tmo_nodeR) && nodemap.TryGetValue(boneL, out tmo_nodeL))
                        {
                            Matrix mR = tmo_nodeR.combined_matrix;
                            Matrix mL = tmo_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, (mR.M43 + mL.M43) / 2.0f);
                        }
                    }
                    break;
                case 4://Shoulder
                    {
                        TMONode tmo_nodeR;
                        TMONode tmo_nodeL;
                        string boneR = "W_RightHand";
                        string boneL = "W_LeftHand";
                        if (nodemap.TryGetValue(boneR, out tmo_nodeR) && nodemap.TryGetValue(boneL, out tmo_nodeL))
                        {
                            Matrix mR = tmo_nodeR.combined_matrix;
                            Matrix mL = tmo_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, (mR.M43 + mL.M43) / 2.0f);
                        }
                    }
                    break;

                default:
                    {
                        TMONode tmo_node;
                        string bone = GetCenterBoneName();
                        if (nodemap.TryGetValue(bone, out tmo_node))
                        {
                            Matrix m = tmo_node.combined_matrix;
                            position = new Vector3(m.M41, m.M42, m.M43);
                        }
                    }
                    break;
            }
            viewer.Camera.Reset();
            viewer.Camera.SetCenter(position);
        }

        public void TranslateToBone(string origin, string bone)
        {
            Vector3 position_origin = new Vector3(0, 0, 0);
            Vector3 position_bone = new Vector3(0, 0, 0);

            TMONode tmo_node;
            Matrix m;

            TMOFile tmo = viewer.FigureList[0].Tmo;

            Dictionary<string, TMONode> nodemap = new Dictionary<string, TMONode>();
            foreach (TMONode node in tmo.nodes)
            {
                if (nodemap.ContainsKey(node.Name) == false)
                    nodemap.Add(node.Name, node);
            }

            if (nodemap.TryGetValue(origin, out tmo_node))
            {
                m = tmo_node.combined_matrix;
                position_origin = new Vector3(m.M41, m.M42, m.M43);
                if (nodemap.TryGetValue(bone, out tmo_node))
                {
                    Vector3 pan_out = new Vector3(0, 0, 10);

                    m = tmo_node.combined_matrix;
                    position_bone = new Vector3(m.M41, m.M42, m.M43);

                    viewer.Camera.SetTranslation(position_bone - position_origin + pan_out);
                }
            }
        }

        public void SetCenter(string bone)
        {
            Vector3 position;

            TMOFile tmo = viewer.FigureList[0].Tmo;

            Dictionary<string, TMONode> nodemap = new Dictionary<string, TMONode>();
            foreach (TMONode node in tmo.nodes)
            {
                if (nodemap.ContainsKey(node.Name) == false)
                    nodemap.Add(node.Name, node);
            }

            TMONode tmo_node;
            if (nodemap.TryGetValue(bone, out tmo_node))
            {
                Matrix m = tmo_node.combined_matrix;
                position = new Vector3(m.M41, m.M42, m.M43);
                viewer.Camera.SetCenter(position);
            }
        }
    }
}
