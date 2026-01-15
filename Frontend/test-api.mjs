// Quick API integration test
import { OpenAPI } from './src/services/payments_api/generated/core/OpenAPI.js';
import { TodoListsService } from './src/services/payments_api/generated/services/TodoListsService.js';
import { TodoItemsService } from './src/services/payments_api/generated/services/TodoItemsService.js';

OpenAPI.BASE = 'http://localhost:5273';

console.log('OpenAPI BASE:', OpenAPI.BASE);

try {
  console.log('\n1. Testing GET /api/todo-lists...');
  const lists = await TodoListsService.getApiTodoLists();
  console.log('✅ Lists:', lists);

  if (lists.length > 0) {
    const listId = lists[0].id;
    console.log(`\n2. Testing GET /api/todo-lists/${listId}/items...`);
    const items = await TodoItemsService.getApiTodoListsItems(listId);
    console.log('✅ Items:', items);
  } else {
    console.log('⚠️ No lists found to test items');
  }
} catch (error) {
  console.error('❌ Error:', error.message);
  console.error('Full error:', error);
}
