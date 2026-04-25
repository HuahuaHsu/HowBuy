import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getStoreStatusApi } from '@/api/store'

export const useSellerStore = defineStore('seller', () => {
  /** 賣場是否被平台停權 */
  const isBanned = ref<boolean>(false)

  /**
   * 從後端拉取賣場停權狀態並存入 store。
   * 若後端尚未提供 isBanned 欄位，暫時回傳 false。
   * TODO: 確認後端 GET /api/front/store/status 有回傳 isBanned: boolean
   */
  async function fetchBanStatus(): Promise<void> {
    try {
      const res = await getStoreStatusApi()
      // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
      isBanned.value = res.data.isBanned ?? false
      if (res.data.isBanned === undefined) {
        // TODO: 需要後端提供 GET /api/front/store/status 回傳 isBanned 欄位
        console.warn('// TODO: 後端 GET /api/front/store/status 尚未回傳 isBanned，目前預設 false')
      }
    } catch {
      isBanned.value = false
    }
  }

  return { isBanned, fetchBanStatus }
})
