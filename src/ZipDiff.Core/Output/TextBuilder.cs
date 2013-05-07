﻿using System.IO;

namespace ZipDiff.Core.Output
{
	class TextBuilder : AbstractBuilder
	{
		public override void Build(StreamWriter writer, Differences diff)
		{
			writer.Write(diff.ToString());
			writer.Flush();
		}
	}
}