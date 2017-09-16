/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSQL
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

namespace Microsoft.EntityFrameworkCore.Scaffolding.Metadata
{
	public class FbIndexModelAnnotations
	{
		private readonly DatabaseIndex _index;

		public FbIndexModelAnnotations(DatabaseIndex index)
		{ 
			_index = index;
		}

		/// <summary>
		/// If the index contains an expression (rather than simple column references), the expression is contained here.
		/// This is currently unsupported and will be ignored.
		/// </summary>
		public string Expression
		{
			get => _index[FbDatabaseModelAnnotationNames.Expression] as string;

			set => _index[FbDatabaseModelAnnotationNames.Expression] = value;
		}
	}
}
