import axios from './axios'
import type { StoreApplyRequest, StoreStatusResponse } from '../types/store'

export const applyStoreApi = (data: StoreApplyRequest) => {
  return axios.post('/api/front/store/apply', data)
}

export const getStoreStatusApi = () => {
  return axios.get<StoreStatusResponse>('/api/front/store/status')
}

export const getSellerDashboardApi = () => {
  return axios.get('/api/front/store/dashboard')
}

export const uploadStoreLogoApi = (file: File) => {
  const formData = new FormData()
  formData.append('file', file)
  return axios.post<{ url: string }>('/api/front/store/upload-logo', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}
