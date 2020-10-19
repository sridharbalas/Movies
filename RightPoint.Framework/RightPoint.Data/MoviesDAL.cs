
using System.Collections;

namespace RightPoint.Data
{
	[System.SerializableAttribute()]
	public partial class MoviesDAL
	{

		#region [ap_Movies_MovieSettings_Get]

		#region Entity

		[System.SerializableAttribute()]
		public partial class MovieSettings_GetRecord
		{
			// GenerateMembersFromResults
			/// <summary>
			/// Gets the ID (System.Int32)
			/// </summary>
			/// <value>The _ID.</value>
			private System.Int32? _ID;

			public System.Int32? ID
			{
				get { return _ID; }
				set { _ID = value; }
			}

			/// <summary>
			/// Gets the Name (System.String)
			/// </summary>
			/// <value>The _Name.</value>
			private System.String _Name;

			public System.String Name
			{
				get { return _Name; }
				set { _Name = value; }
			}

			/// <summary>
			/// Gets the Value (System.String)
			/// </summary>
			/// <value>The _Value.</value>
			private System.String _Value;

			public System.String Value
			{
				get { return _Value; }
				set { _Value = value; }
			}

			/// <summary>
			/// Gets the Description (System.String)
			/// </summary>
			/// <value>The _Description.</value>
			private System.String _Description;

			public System.String Description
			{
				get { return _Description; }
				set { _Description = value; }
			}


			/// <summary>
			/// Initializes a new instance of the <see cref="MovieSettings_GetRecord"/> class.
			/// </summary>
			public MovieSettings_GetRecord(System.Int32? ID,
	 System.String Name,
	 System.String Value,
	 System.String Description)
			{
				_ID = ID;
				_Name = Name;
				_Value = Value;
				_Description = Description;

			}


			/// <summary>
			/// Blank constructor for serialization support only. Please do not use this constructor.
			/// </summary>
			public MovieSettings_GetRecord() { }
		}

		#endregion

		#region Collection

		[System.SerializableAttribute()]
		public partial class MovieSettings_GetRecordCollection : System.Collections.CollectionBase
		{
			public MovieSettings_GetRecordCollection()
			{ }

			public MovieSettings_GetRecordCollection(MovieSettings_GetRecordCollection val)
			{
				this.AddRange(val);
			}

			public MovieSettings_GetRecordCollection(MovieSettings_GetRecord[] val)
			{
				this.AddRange(val);
			}

			public MovieSettings_GetRecord this[int index]
			{
				get { return ((MovieSettings_GetRecord)(List[index])); }
				set { List[index] = value; }
			}

			public int Add(MovieSettings_GetRecord val)
			{
				return List.Add(val);
			}

			public void AddRange(MovieSettings_GetRecord[] val)
			{
				for (int i = 0; i < val.Length; i++)
				{
					this.Add(val[i]);
				}
			}

			public void AddRange(MovieSettings_GetRecordCollection val)
			{
				for (int i = 0; i < val.Count; i++)
				{
					this.Add(val[i]);
				}
			}

			public bool Contains(MovieSettings_GetRecord val)
			{
				return List.Contains(val);
			}

			public void CopyTo(MovieSettings_GetRecord[] array, System.Int32 index)
			{
				List.CopyTo(array, index);
			}

			public int IndexOf(MovieSettings_GetRecord val)
			{
				return List.IndexOf(val);
			}

			public void Insert(int index, MovieSettings_GetRecord val)
			{
				List.Insert(index, val);
			}

			new public MovieSettings_GetRecordEnumerator GetEnumerator()
			{
				return new MovieSettings_GetRecordEnumerator(this);
			}

			public void Remove(MovieSettings_GetRecord val)
			{
				List.Remove(val);
			}

			public void Sort(System.Collections.IComparer comparer)
			{
				InnerList.Sort(comparer);
			}

			protected override void OnInsert(System.Int32 index, System.Object value)
			{
				OnValidate(value);
			}

			protected override void OnRemove(System.Int32 index, System.Object value)
			{
				OnValidate(value);
			}

			protected override void OnSet(System.Int32 index, System.Object oldValue, System.Object newValue)
			{
				OnValidate(oldValue);
				OnValidate(newValue);
			}

			protected override void OnValidate(System.Object value)
			{
				if ((value is MovieSettings_GetRecord) == false)
				{
					throw new System.ArgumentException("value must be of type MovieSettings_GetRecord.");
				}
			}

			public class MovieSettings_GetRecordEnumerator : System.Collections.IEnumerator
			{
				private System.Collections.IEnumerator _baseEnumerator;
				private System.Collections.IEnumerable _temp;

				public MovieSettings_GetRecordEnumerator(MovieSettings_GetRecordCollection mappings)
				{
					_temp = mappings;
					_baseEnumerator = _temp.GetEnumerator();
				}

				public MovieSettings_GetRecord Current
				{
					get { return ((MovieSettings_GetRecord)(_baseEnumerator.Current)); }
				}

				System.Object System.Collections.IEnumerator.Current
				{
					get { return _baseEnumerator.Current; }
				}

				public bool MoveNext()
				{
					return _baseEnumerator.MoveNext();
				}

				public void Reset()
				{
					_baseEnumerator.Reset();
				}
			}
		}

		#endregion
		// ConnectionKey: Movies
		// DatabaseObjectOwner: dbo
		#region Helper Method
		#region Collection Getter Method

		[StoredProcedureName("dbo.ap_Movies_MovieSettings_Get")]
		public static bool TryMovieSettings_Get(out MovieSettings_GetRecordCollection returnValue)
		{
			return WithTransaction.TryMovieSettings_Get(null, out returnValue);
		}

		public partial class WithTransaction
		{
			public static bool TryMovieSettings_Get(System.Data.IDbTransaction transaction, out MovieSettings_GetRecordCollection returnValue)
			{

				int returnCode = MovieSettings_Get(transaction, out returnValue);

				return returnCode == 0;
			}
		}

		private static int MovieSettings_Get(System.Data.IDbTransaction transaction, out MovieSettings_GetRecordCollection returnValue)
		{
			int returnCode = -1;

			returnValue = new MovieSettings_GetRecordCollection();

			System.Data.IDbConnection dbConnection = null;

			try
			{

#if QUERYLOG
		RightPoint.Data.QueryLogItem queryLog = RightPoint.Data.QueryLog.CreateLogItem("Movies.dbo.ap_Movies_MovieSettings_Get");
#endif

				if (transaction != null)
				{
					dbConnection = transaction.Connection;
				}
				else
				{
					dbConnection = RightPoint.Data.DbFactory.GetConnection("Movies");
					dbConnection.Open();
				}

				System.Data.IDbCommand dbCommand = RightPoint.Data.DbUtility.CreateStoredProcedureCommand(dbConnection, "dbo.ap_Movies_MovieSettings_Get");

				if (transaction != null)
					dbCommand.Transaction = transaction;

				dbCommand.CommandTimeout = 120;

				RightPoint.Data.DbUtility.AddReturnParameter(dbCommand);


				System.Data.IDataReader dbDataReader = null;

#if QUERYLOG
		queryLog.MarkExecutionStart();
#endif

				dbDataReader = dbCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

#if QUERYLOG
		queryLog.MarkExecutionEnd();
#endif

				while (dbDataReader.Read())
				{
					returnValue.Add(new MovieSettings_GetRecord((System.Int32?)(dbDataReader.FieldCount < 1 || dbDataReader[0] == System.DBNull.Value ? null : dbDataReader[0]) /* ID */ ,
		 (System.String)(dbDataReader.FieldCount < 2 || dbDataReader[1] == System.DBNull.Value ? null : dbDataReader[1]) /* Name */ ,
		 (System.String)(dbDataReader.FieldCount < 3 || dbDataReader[2] == System.DBNull.Value ? null : dbDataReader[2]) /* Value */ ,
		 (System.String)(dbDataReader.FieldCount < 4 || dbDataReader[3] == System.DBNull.Value ? null : dbDataReader[3]) /* Description */  ));
				}

#if QUERYLOG
		queryLog.MarkTransformationEnd();
#endif

				dbDataReader.Close();
				returnCode = RightPoint.Data.DbUtility.GetReturnCode(dbCommand);

				return returnCode;
			}
			finally
			{
				if (transaction == null && dbConnection != null)
				{
					if (dbConnection.State != System.Data.ConnectionState.Closed)
					{
						dbConnection.Close();
					}

					dbConnection.Dispose();
				}
			}

		}

		#endregion


		#endregion

		#endregion

	}
}

