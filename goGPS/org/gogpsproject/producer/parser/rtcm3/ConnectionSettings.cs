/*
 * Copyright (c) 2010 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
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

namespace org.gogpsproject.producer.parser.rtcm3
{

	using Base64 = org.gogpsproject.util.Base64;

	public class ConnectionSettings
	{

		private string host;
		private int port;
		private string pass_base64;
		private string password;
		private string username;
		private string authbase64;
		private string source;

		public ConnectionSettings(string _host, int _port, string _username, string _password) : base()
		{
			host = _host;
			port = _port;
			password = _password;
			username = _username;
			Authbase64 = username + ":" + password;
		}

		public virtual string Authbase64
		{
			get
			{
				return authbase64;
			}
			set
			{
				this.authbase64 = Base64.encode(value);
			}
		}

		public virtual string Host
		{
			get
			{
				return host;
			}
			set
			{
				this.host = value;
			}
		}

		public virtual string Pass_base64
		{
			get
			{
				return pass_base64;
			}
			set
			{
				this.pass_base64 = value;
			}
		}

		public virtual int Port
		{
			get
			{
				return port;
			}
			set
			{
				this.port = value;
			}
		}

		public virtual string Source
		{
			get
			{
				return source;
			}
			set
			{
				this.source = value;
			}
		}






	}

}