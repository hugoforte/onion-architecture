const { exec, spawn } = require('child_process');
const fs = require('fs');
const path = require('path');
const https = require('https');

// Configure paths relative to your project structure
const apiProjectDir = path.resolve(__dirname, '../../Backend/3_Run/Web');
const uiProjectDir = path.resolve(__dirname, '..');
const swaggerFilePath = path.join(
  uiProjectDir,
  'src',
  'services',
  'payments_api',
  'swagger.json'
);
const openApiTsPath = path.join(
  uiProjectDir,
  'src',
  'services',
  'payments_api',
  'generated',
  'core',
  'OpenAPI.ts'
);
const requestTsPath = path.join(
  uiProjectDir,
  'src',
  'services',
  'payments_api',
  'generated',
  'core',
  'request.ts'
);
const appsettingsPath = path.join(apiProjectDir, 'appsettings.json');

// Configuration
const API_PORT = 5273;
const API_HOST = 'localhost';
const API_PROTOCOL = 'http';
const SWAGGER_ENDPOINT = `${API_PROTOCOL}://${API_HOST}:${API_PORT}/swagger/v1/swagger.json`;

// Core utility functions
const runCommand = (command, options) => {
  return new Promise((resolve, reject) => {
    console.log(`Running: ${command}`);
    exec(command, options, (error, stdout, stderr) => {
      if (error) {
        console.error(`Error executing command: ${command}`);
        console.error(stderr);
        return reject(error);
      }
      if (stdout) console.log(stdout);
      resolve();
    });
  });
};

const downloadFile = (url, dest) => {
  return new Promise((resolve, reject) => {
    const file = fs.createWriteStream(dest);
    const http = require('http');
    const client = url.startsWith('https') ? https : http;
    client
      .get(url, { rejectUnauthorized: false }, response => {
        if (response.statusCode !== 200) {
          reject(
            new Error(`HTTP ${response.statusCode}: ${response.statusMessage}`)
          );
          return;
        }
        response.pipe(file);
        file.on('finish', () => {
          file.close(resolve);
        });
      })
      .on('error', err => {
        fs.unlink(dest, () => reject(err));
      });
  });
};

const waitForApi = async (url, maxAttempts = 5) => {
  let attempts = 0;
  while (attempts < maxAttempts) {
    try {
      await new Promise((resolve, reject) => {
        const http = require('http');
        const client = url.startsWith('https') ? https : http;
        client
          .get(url, { rejectUnauthorized: false }, response => {
            if (response.statusCode === 200) {
              resolve();
            } else {
              reject(new Error(`Status code: ${response.statusCode}`));
            }
          })
          .on('error', reject);
      });
      console.log('✓ API is ready and responding');
      return;
    } catch (error) {
      attempts++;
      console.log(
        `⏳ API not ready, attempt ${attempts}/${maxAttempts}. Retrying in 3 seconds... ${url}`
      );
      await new Promise(resolve => setTimeout(resolve, 3000));
    }
  }
  throw new Error('❌ API did not become ready in time.');
};

const checkRequirements = async () => {
  console.log('🔍 Checking requirements...');

  // Check if in correct directory
  if (!fs.existsSync(path.join(__dirname, '..', 'package.json'))) {
    throw new Error('❌ Must be run from Frontend project directory');
  }

  // Check .NET availability
  await new Promise((resolve, reject) => {
    exec('dotnet --version', (error, stdout) => {
      if (error) {
        reject(new Error('❌ dotnet not found. Please install .NET SDK'));
      }
      console.log(`✓ Found .NET version: ${stdout.trim()}`);
      resolve();
    });
  });

  // Check Git repository
  await new Promise((resolve, reject) => {
    exec('git status', { cwd: uiProjectDir }, error => {
      if (error) {
        reject(new Error('❌ Not in a git repository'));
      }
      console.log('✓ Git repository found');
      resolve();
    });
  });

  // Check NPX availability
  await new Promise((resolve, reject) => {
    exec('npx --version', error => {
      if (error) {
        reject(new Error('❌ npx not found. Please install Node.js'));
      }
      console.log('✓ NPX available');
      resolve();
    });
  });

  // Check API project exists
  if (!fs.existsSync(apiProjectDir)) {
    throw new Error(`❌ API project not found at: ${apiProjectDir}`);
  }
  console.log('✓ API project found');

  console.log('✅ All requirements satisfied');
};

// Custom OpenAPI configuration for payments API
const getOpenApiModifications = () => {
  return `const getHeadersFromGlobalContext = async (): Promise<Headers> => {
  const headers: Headers = {};
  
  // Add Content-Type for requests
  headers["Content-Type"] = "application/json";
  
  // Add Accept header
  headers["Accept"] = "application/json";
  
  // Add CORS headers for development
  // headers["Access-Control-Allow-Origin"] = "*";
  
  // TODO: Add authentication headers when implemented
  // const authToken = getAuthToken();
  // if (authToken) {
  //   headers.Authorization = \`Bearer \${authToken}\`;
  // }
  
  return headers;
};

export const OpenAPI: OpenAPIConfig = {
  BASE: import.meta.env.VITE_API_BASE_URL || "${API_PROTOCOL}://${API_HOST}:${API_PORT}",
  VERSION: "1.0",
  WITH_CREDENTIALS: false,
  CREDENTIALS: "include",
  HEADERS: getHeadersFromGlobalContext,
};
`;
};

const ensureDirectoryExists = dirPath => {
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
    console.log(`📁 Created directory: ${dirPath}`);
  }
};

const main = async () => {
  let apiProcess;
  let originalAppsettingsContent;
  let migrationsDisabled = false;

  try {
    console.log('🚀 Starting Payments API Client Regeneration');
    console.log('='.repeat(50));

    // 1. Validate requirements
    await checkRequirements();

    // 2. Ensure output directories exist
    const outputDir = path.join(
      uiProjectDir,
      'src',
      'services',
      'payments_api'
    );
    ensureDirectoryExists(outputDir);

    // 3. Backup and modify API settings (if needed)
    if (fs.existsSync(appsettingsPath)) {
      originalAppsettingsContent = fs.readFileSync(appsettingsPath, 'utf8');
      console.log('✓ Backed up appsettings.json');

      // Check if ApplyMigrationsOnStartup is set to true and disable it
      try {
        const appsettings = JSON.parse(originalAppsettingsContent);

        if (appsettings.ApplyMigrationsOnStartup === true) {
          console.log(
            '🔧 Disabling ApplyMigrationsOnStartup for API regeneration...'
          );
          appsettings.ApplyMigrationsOnStartup = false;
          fs.writeFileSync(
            appsettingsPath,
            JSON.stringify(appsettings, null, 2),
            'utf8'
          );
          migrationsDisabled = true;
          console.log('✓ ApplyMigrationsOnStartup set to false');
        } else if (appsettings.ApplyMigrationsOnStartup === undefined) {
          console.log(
            '🔧 ApplyMigrationsOnStartup not found, creating entry and setting to false for API regeneration...'
          );
          appsettings.ApplyMigrationsOnStartup = false;
          fs.writeFileSync(
            appsettingsPath,
            JSON.stringify(appsettings, null, 2),
            'utf8'
          );
          migrationsDisabled = true;
          console.log('✓ ApplyMigrationsOnStartup created and set to false');
        }
      } catch (parseError) {
        console.warn(
          '⚠️ Could not parse appsettings.json:',
          parseError.message
        );
        console.warn('⚠️ Continuing without migration modifications');
      }
    }

    // 4. Build API
    console.log('🔨 Building API...');
    await runCommand('dotnet build --configuration Release', {
      cwd: apiProjectDir,
    });

    // 5. Start API in background
    console.log('🚀 Starting API in background...');
    apiProcess = spawn(
      'dotnet',
      [
        'run',
        '--no-build',
        '--configuration',
        'Release',
        '--urls',
        `http://localhost:${API_PORT}`,
      ],
      {
        cwd: apiProjectDir,
        stdio: ['ignore', 'pipe', 'pipe'],
        env: {
          ...process.env,
          AWS_ENDPOINT_URL: 'http://localhost:4566',
          AWS_ACCESS_KEY_ID: 'test',
          AWS_SECRET_ACCESS_KEY: 'test',
          AWS_DEFAULT_REGION: 'us-east-1',
          AWS_REGION: 'us-east-1',
        },
      }
    );

    // Give the API a moment to initialize
    await new Promise(resolve => setTimeout(resolve, 5000));

    // 6. Wait for API readiness
    console.log('⏳ Waiting for API to be ready...');
    await waitForApi(SWAGGER_ENDPOINT);

    // 7. Download Swagger specification
    console.log('📥 Downloading swagger.json...');
    await downloadFile(SWAGGER_ENDPOINT, swaggerFilePath);
    console.log('✓ Swagger specification downloaded');

    // 8. Stop API (we have what we need)
    console.log('🛑 Stopping API...');
    if (apiProcess && !apiProcess.killed) {
      try {
        // Try graceful shutdown first
        apiProcess.kill('SIGTERM');
        console.log('📤 Sent SIGTERM to API process');

        // Wait for graceful shutdown with timeout
        let shutdownComplete = false;
        const shutdownPromise = new Promise(resolve => {
          apiProcess.on('exit', (code, signal) => {
            console.log(
              `✅ API process exited with code ${code}, signal ${signal}`
            );
            shutdownComplete = true;
            resolve();
          });
        });

        // Wait up to 5 seconds for graceful shutdown
        await Promise.race([
          shutdownPromise,
          new Promise(resolve => setTimeout(resolve, 5000)),
        ]);

        // If still running, force kill
        if (!shutdownComplete && !apiProcess.killed) {
          console.log(
            '⚠️ API process did not shutdown gracefully, force killing...'
          );
          apiProcess.kill('SIGKILL');
          await new Promise(resolve => setTimeout(resolve, 1000));
        }

        console.log('✅ API process stopped');
      } catch (error) {
        console.error('⚠️ Error stopping API process:', error.message);
      }
    }

    // 9. Generate client code
    console.log('⚙️ Regenerating API client...');
    const outputPath = path.join(
      uiProjectDir,
      'src',
      'services',
      'payments_api',
      'generated'
    );
    const generateCommand = `npx openapi-typescript-codegen --input "${swaggerFilePath}" --output "${outputPath}" --client axios`;
    await runCommand(generateCommand, { cwd: uiProjectDir });

    // 10. Restore original OpenAPI.ts using git
    console.log('🔄 Restoring original OpenAPI.ts from git...');

    try {
      const relativeOpenApiPath = path
        .relative(uiProjectDir, openApiTsPath)
        .replace(/\\/g, '/');
      await runCommand(`git checkout HEAD -- "${relativeOpenApiPath}"`, {
        cwd: uiProjectDir,
      });
      console.log('✓ Restored original OpenAPI.ts from git');
    } catch (error) {
      console.warn('⚠️ Could not restore OpenAPI.ts from git:', error.message);
      console.warn(
        '   File may not be tracked in git or there were no changes'
      );
    }

    // 11. Format generated code
    console.log('💅 Formatting generated files...');
    try {
      await runCommand(
        `npx prettier --write "src/services/payments_api/**/*.ts"`,
        { cwd: uiProjectDir }
      );
      console.log('✓ Code formatted successfully');
    } catch (error) {
      console.warn('⚠️ Prettier formatting failed, but continuing...');
    }

    // 12. Run npm run format on generated folder
    console.log('🎨 Running npm run format on generated folder...');
    try {
      await runCommand(`npm run format`, { cwd: uiProjectDir });
      console.log('✓ Format completed successfully');
    } catch (error) {
      console.warn('⚠️ npm run format failed, but continuing...');
    }

    // 13. Create index file for easy imports
    const indexPath = path.join(outputDir, 'index.ts');
    const indexContent = `// Auto-generated API client exports
export * from './generated';

// Re-export commonly used types
export type { OpenAPIConfig } from './generated/core/OpenAPI';
`;

    fs.writeFileSync(indexPath, indexContent, 'utf8');
    console.log('✓ Created index file for exports');

    console.log('');
    console.log('🎉 API client regenerated successfully!');
    console.log('='.repeat(50));
    console.log('📂 Generated files location: src/services/payments_api/');
    console.log(
      "📋 You can now import: import { PaymentsApiClient } from 'services/payments_api';"
    );
    console.log('');
  } catch (error) {
    console.error('');
    console.error('❌ Failed to regenerate API client:');
    console.error(error.message);
    console.error('');
    throw error;
  } finally {
    // Cleanup: Restore original settings and kill processes

    // Ensure OpenAPI.ts is restored using git
    try {
      const relativeOpenApiPath = path
        .relative(uiProjectDir, openApiTsPath)
        .replace(/\\/g, '/');
      await runCommand(`git checkout HEAD -- "${relativeOpenApiPath}"`, {
        cwd: uiProjectDir,
      });
      console.log('🔄 Final restoration: OpenAPI.ts restored from git');
    } catch (error) {
      // Silently ignore - file may not have changed or may not be tracked
    }

    if (originalAppsettingsContent && fs.existsSync(appsettingsPath)) {
      try {
        const originalAppsettings = JSON.parse(originalAppsettingsContent);
        const currentAppsettings = JSON.parse(
          fs.readFileSync(appsettingsPath, 'utf8')
        );

        // If we created the entry (it didn't exist originally), set it to true
        if (
          originalAppsettings.ApplyMigrationsOnStartup === undefined &&
          migrationsDisabled
        ) {
          console.log(
            '🔧 ApplyMigrationsOnStartup was created during regeneration, setting to true...'
          );
          currentAppsettings.ApplyMigrationsOnStartup = true;
          fs.writeFileSync(
            appsettingsPath,
            JSON.stringify(currentAppsettings, null, 2),
            'utf8'
          );
          console.log('✓ ApplyMigrationsOnStartup set to true');
        } else if (migrationsDisabled) {
          // Otherwise, restore the original content
          fs.writeFileSync(appsettingsPath, originalAppsettingsContent, 'utf8');
          console.log('🔄 Restored ApplyMigrationsOnStartup to original value');
        }
        console.log('🔄 Reverted appsettings.json to original state');
      } catch (parseError) {
        // Fallback: just restore the original content
        fs.writeFileSync(appsettingsPath, originalAppsettingsContent, 'utf8');
        console.log(
          '🔄 Reverted appsettings.json to original state (fallback)'
        );
      }
    }

    if (apiProcess && !apiProcess.killed) {
      try {
        console.log('🛑 Force-killing API process...');
        apiProcess.kill('SIGKILL');

        // Wait a moment for the kill to take effect
        await new Promise(resolve => setTimeout(resolve, 1000));

        // Check if it's actually killed
        if (!apiProcess.killed) {
          console.log(
            '⚠️ Process still running, trying alternative kill method'
          );
          try {
            process.kill(apiProcess.pid, 'SIGKILL');
          } catch (killError) {
            console.error('⚠️ Could not kill process:', killError.message);
          }
        }

        console.log('🛑 Force-killed API process');
      } catch (e) {
        console.error('⚠️ Error killing API process:', e.message);
      }
    }
  }
};

// Execute the main function
if (require.main === module) {
  main().catch(error => {
    console.error('💥 Script failed:', error.message);
    process.exit(1);
  });
}

module.exports = { main, checkRequirements };
