import axios from './axios';
import type { Order } from '@/types/order';

/**
 * 取得訂單詳情
 * @param id 訂單 ID
 */
export const getOrderDetail = (id: string): Promise<Order> => {
  return axios.get(`/api/orders/${id}`);
};
