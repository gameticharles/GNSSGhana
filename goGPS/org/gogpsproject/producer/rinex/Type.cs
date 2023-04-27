using System;

/*
 * Copyright (c) 2011 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
 *
 * This file is part of goGPS Project (goGPS).
 *
 * goGPS is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3
 * of the License, or (at your option) any later version.
 *
 * goGPS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with goGPS.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
namespace org.gogpsproject.producer.rinex
{
	/// <summary>
	/// <para>
	/// This class holds the data Type config to output in Rinex
	/// </para>
	/// 
	/// @author Lorenzo Patocchi, cryms.com Patocchi cryms.com
	/// </summary>

	/// <summary>
	/// @author Lorenzo Patocchi, cryms.com
	/// 
	/// </summary>
	internal class Type
	{
		public const int C = 1;
		public const int P = 2;
		public const int L = 3;
		public const int D = 4;
		public const int S = 5;
		public const int T = 6;

		public static readonly string[] TYPE_NOTATIONS = new string[] {"","C","P","L","D","S","T"};

		private int type = 0;
		private int frequency = 0;

		public Type(int type, int freq)
		{
			this.type = type;
			this.frequency = freq;
		}

		public override string ToString()
		{
			if (type < 0 || type >= TYPE_NOTATIONS.Length)
			{
				return null;
			}
			return TYPE_NOTATIONS[type] + frequency;
		}

		/// <returns> the type </returns>
		public virtual int Type
		{
			get
			{
				return type;
			}
			set
			{
				this.type = value;
			}
		}


		/// <returns> the frequency </returns>
		public virtual int Frequency
		{
			get
			{
				return frequency;
			}
			set
			{
				this.frequency = value;
			}
		}

	}

}