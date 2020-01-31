using CoreDX.Domain.Repository.App.IdentityServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreDX.Application.Repository.EntityFrameworkCore.IdentityServer.Helpers
{
	public class EnumHelpers
	{
		public static List<SelectItem> ToSelectList<T>() where T : struct, IComparable
		{
			var selectItems = Enum.GetValues(typeof(T))
				.Cast<T>()
				.Select(x => new SelectItem(Convert.ToInt16(x).ToString(), x.ToString())).ToList();

			return selectItems;
		}
	}
}
