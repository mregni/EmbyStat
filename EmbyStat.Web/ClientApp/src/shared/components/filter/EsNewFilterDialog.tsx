import React, {useEffect, useState} from 'react';
import {useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';
import {v4 as uuidv4} from 'uuid';

import {
  Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, MenuItem, Select,
  SelectChangeEvent, Stack, Typography,
} from '@mui/material';

import {useFilterHelpers} from '../../hooks';
import {ActiveFilter, FilterDefinition, FilterOperation} from '../../models/filter';
import {EsFilterInputContainer} from './EsFilterInputContainer';

type EsNewFilterDialogProps = {
  open: boolean;
  handleClose: () => void;
  filters: FilterDefinition[];
  addFilter: (filter: ActiveFilter) => void;
 }

export function EsNewFilterDialog(props: EsNewFilterDialogProps) {
  const {open, handleClose, filters, addFilter} = props;
  const {t} = useTranslation();
  const [fieldId, setFieldId] = useState<string>(filters[0].id);
  const [selectedFilter, setSelectedFilter] = useState<FilterDefinition>(null!);
  const [operationId, setOperationId] = useState<string>('');
  const [selectedOperation, setSelectedOperation] = useState<FilterOperation>(null!);
  const [inputValue, setInputValue] = useState('');
  const {calculateValue, calculateValueLabel} = useFilterHelpers();

  useEffect(() => {
    const index = filters.findIndex((filter) => filter.id === fieldId);
    if (index !== -1) {
      setSelectedFilter(filters[index]);
      setOperationId('');
    }
  }, [fieldId]);

  useEffect(() => {
    if (open) {
      setFieldId(filters[0].id);
    }
  }, [open]);


  useEffect(() => {
    if (selectedFilter !== null) {
      setOperationId(selectedFilter.operations[0].id);
    }
  }, [selectedFilter]);


  useEffect(() => {
    reset();
    if (selectedFilter !== null) {
      const index = selectedFilter.operations.findIndex((operation) => operation.id === operationId);
      if (index !== -1) {
        setSelectedOperation(selectedFilter.operations[index]);
      }
    }
  }, [operationId]);

  const {register, trigger, reset, formState: {errors}} = useForm({
    mode: 'onBlur',
    defaultValues: {
      text: '',
    },
  });

  const onValueChanged = (value: string) => {
    setInputValue(value);
  };

  const handleFieldChange = (event: SelectChangeEvent) => {
    setFieldId(event.target.value as string);
  };

  const handleOperationChange = (event: SelectChangeEvent) => {
    setOperationId(event.target.value as string);
  };

  const handleCreate = async () => {
    if (await trigger()) {
      addFilter({
        field: selectedFilter.field,
        fieldLabel: selectedFilter.label,
        fieldValue: calculateValueLabel(inputValue, selectedOperation),
        operation: selectedOperation,
        value: calculateValue(inputValue, selectedFilter.field),
        id: uuidv4(),
      });
      closeDialog();
      setInputValue('');
    }
  };

  const closeDialog = () => {
    reset();
    handleClose();
  };

  return (
    <Dialog
      open={open}
      onClose={closeDialog}
    >
      <DialogTitle>
        {t('FILTERS.DIALOG.TITLE')}
      </DialogTitle>
      <DialogContent sx={{minWidth: 500}}>
        <Typography sx={{pb: 2}}>
          {t('FILTERS.DIALOG.CONTENT')}
        </Typography>
        <Stack direction="row" spacing={2}>
          <FormControl variant="standard">
            <Select
              value={fieldId}
              onChange={handleFieldChange}
            >
              { filters.map((filter) => ( <MenuItem key={filter.id} value={filter.id}>{t(filter.title)}</MenuItem> )) }
            </Select>
          </FormControl>
          {
            selectedFilter !== null && (
              <FormControl variant="standard">
                <Select
                  value={operationId}
                  onChange={handleOperationChange}
                >
                  {
                    selectedFilter.operations.map((operation) => (
                      <MenuItem key={operation.id} value={operation.id}>{t(operation.label)}</MenuItem>
                    ))
                  }
                </Select>
              </FormControl>
            )
          }
          { selectedOperation !== null && (
            <EsFilterInputContainer
              register={register}
              errors={errors}
              operation={selectedOperation}
              onValueChanged={onValueChanged}
            />)
          }
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={closeDialog} variant="text" color="error">
          {t('COMMON.CANCEL')}
        </Button>
        <Button onClick={handleCreate} autoFocus color="primary" variant="contained">
          {t('COMMON.CREATE')}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
