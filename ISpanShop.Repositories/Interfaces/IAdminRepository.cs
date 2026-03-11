using ISpanShop.Models.DTOs.Admins;
using System.Collections.Generic;

namespace ISpanShop.Repositories.Interfaces
{
	/// <summary>
	/// �޲z����Ʀs������
	/// </summary>
	public interface IAdminRepository
	{
		/// <summary>���o�Ҧ��޲z���]�t����W�١^</summary>
		IEnumerable<AdminDto> GetAllAdmins();

		/// <summary>�� ID ���o��@�޲z��</summary>
		AdminDto? GetAdminById(int adminId);

		/// <summary>��s�޲z������</summary>
		bool UpdateAdminRole(int adminId, int roleId);
	}
}