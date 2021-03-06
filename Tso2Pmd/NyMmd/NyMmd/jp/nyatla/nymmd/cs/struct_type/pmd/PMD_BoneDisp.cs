﻿/* 
 * PROJECT: MMD for Java
 * --------------------------------------------------------------------------------
 * This work is based on the ARTK_MMD v0.1 
 *   PY
 * http://ppyy.hp.infoseek.co.jp/
 * py1024<at>gmail.com
 * http://www.nicovideo.jp/watch/sm7398691
 *
 * The MMD for Java is Java version MMD class library.
 * Copyright (C)2009 nyatla
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this framework; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * For further information please contact.
 *	http://nyatla.jp/
 *	<airmail(at)ebony.plala.or.jp>
 * 
 */
using System;
using System.Text;
using System.IO;
using jp.nyatla.nymmd.cs.types;

namespace jp.nyatla.nymmd.cs.struct_type.pmd
{
    public class PMD_BoneDisp : StructType
    {
        internal int bone_index; // 枠用ボーン番号
        public int bone_disp_frame_index; // 表示枠番号

        public void read(DataReader i_reader)
        {
            this.bone_index = i_reader.readUnsignedShort(); // 枠用ボーン番号
            this.bone_disp_frame_index = i_reader.readByte(); // 表示枠番号

            return;
        }

        public void write(DataWriter i_writer)
        {
            i_writer.writeUnsignedShort(this.bone_index); // 枠用ボーン番号
            i_writer.writeByte(this.bone_disp_frame_index); // 表示枠番号

            return;
        }

        // 以下は入出力とは関係ない便宜上のデータ
        // 外部からは名前で処理を行い、入出力のときのみ番号で処理する
        public string bone_name;
    }

}
