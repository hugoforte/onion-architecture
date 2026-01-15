import axios, { AxiosInstance } from 'axios';

const apiBaseUrl = 'http://localhost:5273';

const client: AxiosInstance = axios.create({
  baseURL: `${apiBaseUrl}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
});

export default client;
