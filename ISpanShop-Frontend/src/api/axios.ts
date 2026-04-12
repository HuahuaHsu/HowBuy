import axios from 'axios';
import { ElMessage } from 'element-plus';

const instance = axios.create({
  // 請根據您的 WebAPI 實際 Port 調整，例如 http://localhost:7001
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  timeout: 10000,
});

// 回應攔截器：處理錯誤訊息
instance.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const message = error.response?.data?.message || '網路錯誤，請稍後再試';
    ElMessage.error(message);
    return Promise.reject(error);
  }
);

export default instance;
