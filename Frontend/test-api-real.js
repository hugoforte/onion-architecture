import { todoApi } from './src/api/todoApi.js';

async function testApi() {
  console.log('=== Testing Todo API ===\n');
  
  try {
    // Test 1: Get lists
    console.log('1️⃣  Getting todo lists...');
    const lists = await todoApi.getTodoLists();
    console.log('✅ Lists fetched:', lists.length, 'lists found');
    console.log('   Sample:', lists[0] ? { id: lists[0].id, name: lists[0].name } : 'none');
    
    // Test 2: Get items from first list
    if (lists.length > 0) {
      console.log('\n2️⃣  Getting todo items from first list...');
      const items = await todoApi.getTodoItems(lists[0].id!);
      console.log('✅ Items fetched:', items.length, 'items found');
      console.log('   Sample:', items[0] ? { id: items[0].id, title: items[0].title } : 'none');
    } else {
      console.log('\n2️⃣  Creating a test list first...');
      const newList = await todoApi.createTodoList({
        name: 'API Test List',
        description: 'Testing API integration',
      });
      console.log('✅ List created:', { id: newList.id, name: newList.name });
      
      console.log('\n3️⃣  Creating a test item...');
      const newItem = await todoApi.createTodoItem({
        todoListId: newList.id!,
        title: 'Test Item',
        description: 'Testing API item creation',
      });
      console.log('✅ Item created:', { id: newItem.id, title: newItem.title });
      
      console.log('\n4️⃣  Getting items from test list...');
      const testItems = await todoApi.getTodoItems(newList.id!);
      console.log('✅ Items fetched:', testItems.length, 'items found');
    }
    
    console.log('\n✅ All API tests passed!');
    return true;
  } catch (error: any) {
    console.error('\n❌ API Test Failed!');
    console.error('Error:', error.message);
    if (error.response) {
      console.error('Status:', error.response.status);
      console.error('Data:', error.response.data);
    }
    if (error.config) {
      console.error('Request URL:', error.config.url);
      console.error('Request Method:', error.config.method);
    }
    return false;
  }
}

export { testApi };
