import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Grid,
  Card,
  CardContent,
  CardActions,
  Typography,
  CircularProgress,
  Alert,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { todoApi, CreateTodoListDto } from '@/api/todoApi';
import { useTodoStore } from '@/store/todoStore';
import TodoItemsView from '@/features/todoItems/TodoItemsView';

function TodoListsView() {
  const queryClient = useQueryClient();
  const [openDialog, setOpenDialog] = useState(false);
  const [formData, setFormData] = useState<CreateTodoListDto>({
    name: '',
    description: '',
  });
  const { selectedListId, setSelectedListId } = useTodoStore();

  const {
    data: lists,
    isLoading,
    error,
  } = useQuery({
    queryKey: ['todoLists'],
    queryFn: () => todoApi.getTodoLists(),
  });

  const createMutation = useMutation({
    mutationFn: (dto: CreateTodoListDto) => todoApi.createTodoList(dto),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todoLists'] });
      setFormData({ name: '', description: '' });
      setOpenDialog(false);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => todoApi.deleteTodoList(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todoLists'] });
      setSelectedListId(null);
    },
  });

  const handleCreateClick = () => {
    setOpenDialog(true);
  };

  const handleDialogClose = () => {
    setOpenDialog(false);
    setFormData({ name: '', description: '' });
  };

  const handleCreate = async () => {
    await createMutation.mutateAsync(formData);
  };

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this todo list?')) {
      await deleteMutation.mutateAsync(id);
    }
  };

  if (isLoading) return <CircularProgress />;
  if (error) return <Alert severity="error">Failed to load todo lists</Alert>;

  return (
    <Box>
      <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4">My Todo Lists</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={handleCreateClick}>
          Create List
        </Button>
      </Box>

      <Grid container spacing={2}>
        {lists?.map((list) => (
          <Grid item xs={12} sm={6} md={4} key={list.id}>
            <Card
              sx={{
                cursor: 'pointer',
                border: selectedListId === list.id ? '2px solid' : '1px solid',
                borderColor: selectedListId === list.id ? 'primary.main' : 'divider',
              }}
              onClick={() => list.id && setSelectedListId(list.id)}
            >
              <CardContent>
                <Typography variant="h6">{list.name}</Typography>
                <Typography variant="body2" color="textSecondary">
                  {list.description}
                </Typography>
              </CardContent>
              <CardActions>
                <Button size="small" startIcon={<EditIcon />}>
                  Edit
                </Button>
                <Button
                  size="small"
                  startIcon={<DeleteIcon />}
                  onClick={(e) => {
                    e.stopPropagation();
                    if (list.id) handleDelete(list.id);
                  }}
                >
                  Delete
                </Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>

      {selectedListId && <TodoItemsView todoListId={selectedListId} />}

      <Dialog open={openDialog} onClose={handleDialogClose}>
        <DialogTitle>Create Todo List</DialogTitle>
        <DialogContent sx={{ minWidth: 400 }}>
          <TextField
            autoFocus
            margin="dense"
            label="Name"
            fullWidth
            variant="outlined"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Description"
            fullWidth
            variant="outlined"
            multiline
            rows={3}
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleDialogClose}>Cancel</Button>
          <Button onClick={handleCreate} variant="contained">
            Create
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default TodoListsView;
