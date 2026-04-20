export interface StoreApplyRequest {
  storeName: string
  description?: string
  logoUrl?: string
}

export type StoreStatus = 'NotApplied' | 'Pending' | 'Approved' | 'Rejected'

export interface StoreStatusResponse {
  status: StoreStatus
}
