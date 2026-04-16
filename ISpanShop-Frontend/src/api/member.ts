import axios from './axios';

/** 會員資料 DTO (用於讀取) */
export interface MemberDto {
  id: number;
  account: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  gender: number | null;
  birthday: string | null;
  pointBalance?: number;
  levelName?: string;
  avatarUrl?: string;
}

/** 更新會員資料 DTO (用於寫入) */
export interface UpdateMemberProfileDto {
  id: number;
  account: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  gender: number | null;
  birthday: string | null;
  avatarUrl?: string;
}

/**
 * 取得會員個人資料
 */
export const getMemberProfile = (id: number) => {
  return axios.get<MemberDto>(`/api/front/profile/${id}`);
};

/**
 * 更新會員個人資料
 */
export const updateMemberProfile = (id: number, data: UpdateMemberProfileDto) => {
  return axios.put<{ message: string }>(`/api/front/profile/${id}`, data);
};
