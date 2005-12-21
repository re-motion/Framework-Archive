function TypeUtility_IsObject (value)
{
  return typeof (value) == 'object';    
}

function TypeUtility_IsString (value)
{
  return typeof (value) == 'string';    
}

function TypeUtility_IsNumber (value)
{
  return typeof (value) == 'number';    
}

function TypeUtility_IsBoolean (value)
{
  return typeof (value) == 'boolean';    
}

function TypeUtility_IsFunction (value)
{
  return typeof (value) == 'function';    
}

function TypeUtility_IsUndefined (value)
{
  return typeof (value) == 'undefined';    
}

function TypeUtility_IsNull (value)
{
  return ! TypeUtility_IsUndefined (value) && value == null;    
}

function StringUtility_IsNullOrEmpty (value)
{
  ArgumentUtility_CheckTypeIsString ('value', value);
  return TypeUtility_IsNull (value) || value.length == 0;    
}

// Checks that value is not null.
function ArgumentUtility_CheckNotNull (name, value)
{
  if (TypeUtility_IsNull (value))
    throw ('Error: The value of parameter "' + name + '" is null.');
}

// Checks that value is not null and of type string.
function ArgumentUtility_CheckTypeIsString (name, value)
{
  if (TypeUtility_IsNull (value))
    return;
  if (! TypeUtility_IsString (value))
    throw ('Error: The value of parameter "' + name + '" is not a string.');
}

// Checks that value is not null and of type string.
function ArgumentUtility_CheckNotNullAndTypeIsString (name, value)
{
  ArgumentUtility_CheckNotNull (name, value);
  ArgumentUtility_CheckTypeIsString (name, value);
}

// Checks that value is not null and of type string.
function ArgumentUtility_CheckTypeIsObject (name, value)
{
  if (TypeUtility_IsNull (value))
    return;
  if (! TypeUtility_IsObject (value))
    throw ('Error: The value of parameter "' + name + '" is not an object.');
}

// Checks that value is not null and of type string.
function ArgumentUtility_CheckNotNullAndTypeIsObject (name, value)
{
  ArgumentUtility_CheckNotNull (name, value);
  ArgumentUtility_CheckTypeIsObject (name, value);
}

// Checks that value is not null and of type number.
function ArgumentUtility_CheckTypeIsNumber (name, value)
{
  if (TypeUtility_IsNull (value))
    return;
  if (! TypeUtility_IsNumber (value))
    throw ('Error: The value of parameter "' + name + '" is not a number.');
}

// Checks that value is not null and of type number.
function ArgumentUtility_CheckNotNullAndTypeIsNumber (name, value)
{
  ArgumentUtility_CheckNotNull (name, value);
  ArgumentUtility_CheckTypeIsNumber (name, value);
}

// Checks that value is not null and of type boolean.
function ArgumentUtility_CheckTypeIsBoolean (name, value)
{
  if (TypeUtility_IsNull (value))
    return;
  if (! TypeUtility_IsBoolean (value))
    throw ('Error: The value of parameter "' + name + '" is not a boolean.');
}

// Checks that value is not null and of type boolean.
function ArgumentUtility_CheckNotNullAndTypeIsBoolean (name, value)
{
  ArgumentUtility_CheckNotNull (name, value);
  ArgumentUtility_CheckTypeIsBoolean (name, value);
}

// Checks that value is not null and of type function.
function ArgumentUtility_CheckTypeIsFunction (name, value)
{
  if (TypeUtility_IsNull (value))
    return;
  if (! TypeUtility_IsFunction (value))
    throw ('Error: The value of parameter "' + name + '" is not a function.');
}

// Checks that value is not null and of type function.
function ArgumentUtility_CheckNotNullAndTypeIsFunction (name, value)
{
  ArgumentUtility_CheckNotNull (name, value);
  ArgumentUtility_CheckTypeIsFunction (name, value);
}
