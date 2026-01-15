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
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Checkbox,
  IconButton,
  Typography,
  CircularProgress,
  Alert,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import { todoApi, CreateTodoItemDto } from '@/api/todoApi';

interface TodoItemsViewProps {
  todoListId: string;
}

function TodoItemsView({ todoListId }: TodoItemsViewProps) {
  const queryClient = useQueryClient();
  const [openDialog, setOpenDialog] = useState(false);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
  });

  const {
    data: items,
    isLoading,
    error,
  } = useQuery({
    queryKey: ['todoItems', todoListId],
    queryFn: () => todoApi.getTodoItems(todoListId),
  });

  const createMutation = useMutation({
    mutationFn: (dto: CreateTodoItemDto) => todoApi.createTodoItem(dto),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todoItems', todoListId] });
      setFormData({ title: '', description: '' });
      setOpenDialog(false);
    },
  });

  const completeMutation = useMutation({
    mutationFn: (id: string) => todoApi.completeTodoItem(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todoItems', todoListId] });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => todoApi.deleteTodoItem(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todoItems', todoListId] });
    },
  });

  const handleCreateClick = () => {
    setOpenDialog(true);
  };

  const handleDialogClose = () => {
    setOpenDialog(false);
    setFormData({ title: '', description: '' });
  };

  const handleCreate = async () => {
    const dto: CreateTodoItemDto = {
      ...formData,
      todoListId,
    };
    await createMutation.mutateAsync(dto);
  };

  const handleComplete = async (id: string) => {
    await completeMutation.mutateAsync(id);
  };

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this item?')) {
      await deleteMutation.mutateAsync(id);
    }
  };

  if (isLoading) return <CircularProgress />;
  if (error) return <Alert severity="error">Failed to load todo items</Alert>;

  return (
    <Box sx={{ mt: 4 }}>
      <Box sx={{ mb: 2, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h6">Todo Items</Typography>
        <Button
          variant="contained"
          size="small"
          startIcon={<AddIcon />}
          onClick={handleCreateClick}
        >
          Add Item
        </Button>
      </Box>

      <List>
        {items?.map((item) => (
          <ListItem
            key={item.id}
            disablePadding
            secondaryAction={
              <IconButton
                edge="end"
                aria-label="delete"
                onClick={() => item.id && handleDelete(item.id)}
              >
                <DeleteIcon />
              </IconButton>
            }
          >
            <ListItemButton
              role={undefined}
              onClick={() => item.id && handleComplete(item.id)}
              dense
            >
              <ListItemIcon>
                <Checkbox edge="start" checked={item.isCompleted} tabIndex={-1} disableRipple />
              </ListItemIcon>
              <ListItemText
                primary={item.title}
                secondary={item.description}
                sx={{
                  textDecoration: item.isCompleted ? 'line-through' : 'none',
                  opacity: item.isCompleted ? 0.6 : 1,
                }}
              />
            </ListItemButton>
          </ListItem>
        ))}
      </List>

      <Dialog open={openDialog} onClose={handleDialogClose}>
        <DialogTitle>Add Todo Item</DialogTitle>
        <DialogContent sx={{ minWidth: 400 }}>
          <TextField
            autoFocus
            margin="dense"
            label="Title"
            fullWidth
            variant="outlined"
            value={formData.title}
            onChange={(e) => setFormData({ ...formData, title: e.target.value })}
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

export default TodoItemsView;
