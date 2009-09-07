﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Media;

namespace TDCGExplorer
{
    public class LbGenItem : Object
    {
        public virtual void DoClick()
        {
        }

        public virtual void DoTahEdit()
        {
        }

        public virtual void DoDecrypt()
        {
        }

        public virtual void DoDeleteTahEdit()
        {
        }
    }

    public static class LBFileTahUtl
    {
        public static void OpenTahEditor(GenericTahInfo entry)
        {
            string tahdbpath = GetTahDbPath(entry);
            // 同じDBエントリを開いているエディタタブが存在しないかチェックする。
            foreach (TabPage tabpage in TDCGExplorer.MainFormWindow.TabControlMainView.Controls)
            {
                TAHEditor edit = tabpage.Controls[0] as TAHEditor;
                if (edit != null)
                {
                    if (edit.TahDBPath == tahdbpath)
                    {
                        MessageBox.Show("既にこのTAHを開いています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
            }
            // 既にファイルが存在しないかチェックする.
            if (File.Exists(tahdbpath))
            {
                string path;
                using(TAHLocalDB tahdb = new TAHLocalDB())
                {
                    tahdb.Open(tahdbpath);
                    path = tahdb["source"];
                }
                if(path!=entry.path){
                    if(MessageBox.Show("別のTAHファイルを格納しているDBがあります。\n削除して新規作成しますか？", "DBの更新", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        // ファイルを削除する.
                        File.Delete(tahdbpath);
                        // 新規作成する.
                        TAHEditor editor = new TAHEditor(tahdbpath,new GenericZipsTahInfo(entry));
                        TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                        editor.SelectAll();
                    }
                }else{
                    // 既にあるファイルをオープンする.
                    TAHEditor editor = new TAHEditor(tahdbpath, null);
                    TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                    editor.SelectAll();
                }
            }
            else
            {
                // 新規に作成する.
                TAHEditor editor = new TAHEditor(tahdbpath, new GenericZipsTahInfo(entry));
                TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                editor.SelectAll();
            }
        }
        public static void DeleteTahEditorFile(GenericTahInfo entry)
        {
            File.Delete(GetTahDbPath(entry));
        }
        public static string GetTahDbPath(GenericTahInfo entry)
        {
            return (Path.Combine(TDCGExplorer.SystemDB.tahpath, entry.shortname) + ".db").ToLower();
        }
        public static string GetTahDbPath(string localpath)
        {
            return (Path.Combine(TDCGExplorer.SystemDB.tahpath, localpath) + ".db").ToLower();
        }
    }

    public class LbFileItem : LbGenItem
    {
        ArcsTahEntry entry;
        public LbFileItem(ArcsTahEntry argentry)
        {
            entry = argentry;
        }
        public override string ToString()
        {
            return Path.GetFileName(entry.path);
        }
        public override void DoClick()
        {
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    TDCGExplorer.MainFormWindow.AssignTagPageControl(new TAHPageControl(new GenericArcsTahInfo(entry), TDCGExplorer.ArcsDB.GetTahFilesPath(entry.id)));
                    break;
            }
        }
        public override void DoDecrypt()
        {
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    TDCGExplorer.TAHDecrypt(new GenericArcsTahInfo(entry));
                    break;
                default:
                    MessageBox.Show("TAHファイル以外は展開できません.", "TAHDecrypt", MessageBoxButtons.OK);
                    break;
            }
        }
        public override void DoTahEdit()
        {
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    LBFileTahUtl.OpenTahEditor(new GenericArcsTahInfo(entry));
                    break;
                default:
                    MessageBox.Show("TAHファイル以外は編集できません.", "TAHDecrypt", MessageBoxButtons.OK);
                    return;
            }
        }
        public override void DoDeleteTahEdit()
        {
            // TAH編集ファイルを削除する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    LBFileTahUtl.DeleteTahEditorFile(new GenericArcsTahInfo(entry));
                    break;
                default:
                    MessageBox.Show("TAHファイル以外は編集できません.", "TAHDecrypt", MessageBoxButtons.OK);
                    return;
            }
        }
    }

    public class LbCollisionItem : LbGenItem
    {
        CollisionItem entry;
        public LbCollisionItem(CollisionItem argentry)
        {
            entry = argentry;
        }
        public override string ToString()
        {
            return Path.GetFileName(entry.tah.path);
        }
        public override void DoClick()
        {
            switch (Path.GetExtension(entry.tah.shortname))
            {
                case ".tah":
                    TDCGExplorer.MainFormWindow.AssignTagPageControl(new CollisionTahPageControl(entry));
                    break;
            }
        }
    }

    public class LbZipFileItem : LbGenItem
    {
        ArcsZipTahEntry entry;
        public LbZipFileItem(ArcsZipTahEntry argentry)
        {
            entry = argentry;
        }
        public override string ToString()
        {
            return entry.path;
        }
        public override void DoClick()
        {
            // セーブファイルか?
            string savefilpath = entry.path.ToLower();
            if (savefilpath.EndsWith(".tdcgsav.png") || savefilpath.EndsWith(".tdcgsav.bmp"))
            {
                TDCGExplorer.MainFormWindow.AssignTagPageControl(new SaveFilePage(new GenericZipsTahInfo(entry)));
                return;
            }
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.path.ToLower()))
            {
                case ".tah":
                    TDCGExplorer.MainFormWindow.AssignTagPageControl(new TAHPageControl(new GenericZipsTahInfo(entry), TDCGExplorer.ArcsDB.GetZipTahFilesEntries(entry.id)));
                    break;
                case ".bmp":
                case ".png":
                case ".jpg":
                case ".gif":
                case ".tif":
                    TDCGExplorer.MainFormWindow.AssignTagPageControl(new ImagePageControl(new GenericZipsTahInfo(entry)));
                    break;

                case ".txt":
                case ".doc":
                case ".xml":
                    TDCGExplorer.MainFormWindow.AssignTagPageControl(new TextPageControl(new GenericZipsTahInfo(entry)));
                    break;
            }
        }
        public override void DoTahEdit()
        {
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    LBFileTahUtl.OpenTahEditor(new GenericZipsTahInfo(entry));
                    break;
                default:
                    MessageBox.Show("TAHファイル以外は編集できません.", "TAHDecrypt", MessageBoxButtons.OK);
                    return;
            }
        }

        public override void DoDecrypt()
        {
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    TDCGExplorer.TAHDecrypt(new GenericZipsTahInfo(entry));
                    break;
                default:
                    MessageBox.Show("TAHファイル以外は展開できません.", "TAHDecrypt", MessageBoxButtons.OK);
                    break;
            }
        }
        public override void DoDeleteTahEdit()
        {
            // TAH編集ファイルを削除する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    LBFileTahUtl.DeleteTahEditorFile(new GenericZipsTahInfo(entry));
                    break;
                default:
                    MessageBox.Show("TAHファイル以外は編集できません.", "TAHDecrypt", MessageBoxButtons.OK);
                    return;
            }
        }

    }
    // セーブファイル専用リストボックスアイテム.
    public class LbSaveFileItem : LbGenItem
    {
        string path;
        public LbSaveFileItem(string itpath)
        {
            path = itpath;
        }
        public override string ToString()
        {
            return Path.GetFileName(path);
        }
        public override void DoClick()
        {
            TDCGExplorer.MainFormWindow.AssignTagPageControl(new SaveFilePage(path));
        }
    }

}