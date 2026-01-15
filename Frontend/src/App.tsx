import { Container, Box, AppBar, Toolbar, Typography } from '@mui/material';
import TodoListsView from '@/features/todoLists/TodoListsView';

function App() {
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Todo App
          </Typography>
        </Toolbar>
      </AppBar>
      <Container maxWidth="md">
        <Box sx={{ py: 4 }}>
          <TodoListsView />
        </Box>
      </Container>
    </>
  );
}

export default App;
