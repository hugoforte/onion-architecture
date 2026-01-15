import React from 'react';
import ReactDOM from 'react-dom/client';
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import { QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { OpenAPI } from '@/services/payments_api';
import App from '@/App';
import '@/index.css';

// Configure OpenAPI client before any API calls
OpenAPI.BASE = 'http://localhost:5273';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 1000 * 60 * 5, // 5 minutes
      gcTime: 1000 * 60 * 10, // 10 minutes (formerly cacheTime)
    },
    mutations: {
      retry: 1,
    },
  },
});

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <App />
      </ThemeProvider>
    </QueryClientProvider>
  </React.StrictMode>
);
